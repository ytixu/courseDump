#ifndef MMN_SKELETONLOKALISER_HPP_1403
#define MMN_SKELETONLOKALISER_HPP_1403

#include "MCLocaliser.hpp"

// Used for simulateRangeScan
#include "occupancy_grid_utils/ray_tracer.h"

#include <iostream>
#include <boost/random.hpp>
#include <boost/random/normal_distribution.hpp>

#include <cmath>

class SkeletonLocaliser: public MCLocaliser
{
public:
  const static double degree = 180.0/M_PI;
  const static double radian = M_PI/180.0;
  const static double TWO_PI = M_PI*2;

  const double angularDist(double aRad, double bRad)
  {
    double dRad = bRad - aRad + M_PI;
    if (dRad > 0) {
      dRad = dRad - floor(dRad/TWO_PI)*TWO_PI - M_PI;
    } else {
      dRad = dRad - (floor(dRad/TWO_PI) + 1)*TWO_PI + M_PI;
    }
    return abs(dRad);
  }

  // TODO: these constants may need to be tuned
  const static double INIT_XY_STD_M = 15.0;
  const static double INIT_YAW_STD_DEG = 10000.0; // large value -> uniform random

  // parameters for the odometer, taken from lab.world
  const static double ODOM_X_DEV = 0.2;
  const static double ODOM_Y_DEV = 0.2;
  const static double ODOM_YAW_DEV = 0.2;

  // for sensorModel
  const static int MAX_UNMATCH = 5;
  const static double TRSHLD = 0.5;
  const static int RANGE_SIZE = 180; // taken from lab.world

  // Hard-coded world constraints (okay for this assignment)
  const static double WORLD_X_MIN = 0;
  const static double WORLD_X_MAX = 50;
  const static double WORLD_Y_MIN = 0;
  const static double WORLD_Y_MAX = 50;

  // for resampling 
  double MIN_N_EFF = 0;
  int PARTICLE_NUM = 0;

  boost::mt19937 rng;
  boost::normal_distribution<> gausNorm;
  boost::uniform_real<> uniformReal;
  boost::variate_generator<boost::mt19937&, boost::normal_distribution<> > randg;
  boost::variate_generator<boost::mt19937&, boost::uniform_real<> > randn;
  
  
  SkeletonLocaliser( int particleCount = 400 ) : MCLocaliser(particleCount), 
    gausNorm(0.0, 1.0), uniformReal(0.0, 1.0), 
    randg(rng, gausNorm), randn(rng, uniformReal)
  {
    PARTICLE_NUM = particleCount;
    // this is threshold for N_eff
    MIN_N_EFF = particleCount/1.5;
  }

  /**
  * Helper method to set the pose of a particle in particleCloud.
  */
  void setPose(int i, double w, double x, double y, double t)
  {
    this->particleCloud.poses[i].position.x = x;
    this->particleCloud.poses[i].position.y = y;
    this->particleCloud.poses[i].orientation = tf::createQuaternionMsgFromYaw(fmod(t, TWO_PI));
    this->particleCloud.poses[i].position.z = w;
  }

  /**
  * Method to compare the poses according to their weight
  * Used to sort the poses
  */
  static bool comparePose(const geometry_msgs::Pose& a, const geometry_msgs::Pose& b)
  {
    return a.position.z < b.position.z;
  }

  virtual void initialisePF( const geometry_msgs::PoseWithCovarianceStamped& initialpose )
  {  
    double xInitM = initialpose.pose.pose.position.x;
    double yInitM = initialpose.pose.pose.position.y;
    double yawInitRad = tf::getYaw(initialpose.pose.pose.orientation);
    ROS_INFO("Initializing PF at (x, y)=(%.2f, %.2f) m and yaw=%.2f deg", xInitM, yInitM, yawInitRad*degree);
    
    // generate particles from uniform distribution
    const double initWeight = 1.0/PARTICLE_NUM;
    for (unsigned int i = 0; i < PARTICLE_NUM; ++i)
    {
      setPose(i, initWeight, WORLD_X_MAX*randn(), WORLD_Y_MAX*randn(), i*0.01 + randg());
    }
  }
  
  
protected:

  /**
   * Generate new sample from p(x_[t]|x_[t-1], u_[t-1]) which is 
   * normal with mean x_[t-1] + mean_u and variance variance_u
   * Do this for x, y, t
   */
  virtual void applyMotionModel( double deltaX, double deltaY, double deltaT )
  {
    if (fabs(deltaX) > 0 or fabs(deltaY) > 0 or fabs(deltaT) > 0){
      ROS_INFO( "applying odometry: %f %f %f", deltaX, deltaY, deltaT );
      for (unsigned int i = 0; i < PARTICLE_NUM; ++i)
      {
        this->particleCloud.poses[i].position.x = randg()*ODOM_X_DEV 
          + this->particleCloud.poses[i].position.x + deltaX;
        this->particleCloud.poses[i].position.y = randg()*ODOM_Y_DEV 
          + this->particleCloud.poses[i].position.y + deltaY;
        this->particleCloud.poses[i].orientation = tf::createQuaternionMsgFromYaw(fmod(randg()*ODOM_YAW_DEV 
          + tf::getYaw(this->particleCloud.poses[i].orientation) + deltaT, TWO_PI));
      }
    }
  }
  

  /**
   */
  virtual void applySensorModel( const sensor_msgs::LaserScan& scan )
  {
    geometry_msgs::Pose sensor_pose;
    sensor_msgs::LaserScan::Ptr simulatedScan;

    double totWeight = 0.0;
    for (unsigned int i = 0; i < PARTICLE_NUM; ++i)
    {
      sensor_pose =  this->particleCloud.poses[i];
      /* If the laser and centre of the robot weren't at the same
       * position, we would first apply the tf from /base_footprint
       * to /base_laser here. */
      try{
        simulatedScan
          = occupancy_grid_utils::simulateRangeScan
          ( this->map, sensor_pose, scan, true );
      }
      catch (occupancy_grid_utils::PointOutOfBoundsException)
      {
        this->particleCloud.poses[i].position.z = 0.0;
        continue;
      }

      // // heuristic: match min and max ranges
      // if (fabs((*simulatedScan).range_min - scan.range_min) > TRSHLD ||
      //     fabs((*simulatedScan).range_max - scan.range_max) > TRSHLD)
      // {
      //   this->particleCloud.poses[i].position.z = 0.0;
      //   continue;
      // }

      double pr = 0;
      for (unsigned int j = 0; j<RANGE_SIZE; j++)
      {
        pr += exp(- pow((scan.ranges[j] - (*simulatedScan).ranges[j]) / TRSHLD, 2));
      }
      pr /= (1-this->particleCloud.poses[i].position.z);
      this->particleCloud.poses[i].position.z = pr;
      totWeight += pr;
    }
    ROS_INFO("Normalizer: %f", totWeight);
    // normalize
    for (unsigned int i = 0; i < PARTICLE_NUM; ++i)
    {
      this->particleCloud.poses[i].position.z /= totWeight;
    }
  }


/**
  * Resampling algorithm based on this paper
  * http://www.cse.psu.edu/~rcollins/CSE598G/papers/ParticleFilterTutorial.pdf
  */
  virtual geometry_msgs::PoseArray updateParticleCloud
  ( const sensor_msgs::LaserScan& scan,
    const nav_msgs::OccupancyGrid& map,
    const geometry_msgs::PoseArray& particleCloud )
  {
    // estimate N_eff with sum w^2
    double weight_sqrd = 0.0;
    for (unsigned int i = 0; i < PARTICLE_NUM; ++i)
    {
      weight_sqrd += pow(this->particleCloud.poses[i].position.z, 2);
    }
    ROS_INFO("N_eff = %f", 1 / weight_sqrd);    
    // check N_eff
    if (1 / weight_sqrd > MIN_N_EFF){
      return this->particleCloud;
    }
    // else resample
    sort (this->particleCloud.poses.begin(), this->particleCloud.poses.end(), comparePose);
    // starting cdf at 0
    double cdf = 0;
    unsigned int i = 0;
    // draw samples
    double u = randn()/PARTICLE_NUM;
    double uj = 0.0;
    for (unsigned int j = 0; j < PARTICLE_NUM; ++j)
    {  
      uj = u + j/((float)PARTICLE_NUM);
      while (uj > cdf)
      {
        if (i >= PARTICLE_NUM)
        {
          cdf = 1.0;
          i--;
        }
        else
        {
          cdf += this->particleCloud.poses[i].position.z;
          i++;
        }
      }
      setPose(j, 1/PARTICLE_NUM, 
        randg() + this->particleCloud.poses[i].position.x,
        randg() + this->particleCloud.poses[i].position.y,
        randg()*ODOM_YAW_DEV + tf::getYaw(this->particleCloud.poses[i].orientation));
    }
    return this->particleCloud;
  }

  /**
   * Most likely pose = sample mean
   * Assums that x,y,yaw are independent
   * Variance = corrected sample variance = 1/(n-1) SUM difference squared ~ 1/n SUM difference squared for large n
   * For efficiency, we will estimate the variance using the previous estimate. 
   */
  virtual geometry_msgs::PoseWithCovariance updatePose()
  {
    double x = 0.0;
    double y = 0.0;
    double t = 0.0;
    this->estimatedPose.pose.covariance[0] = 0.0;
    this->estimatedPose.pose.covariance[7] = 0.0;
    this->estimatedPose.pose.covariance[35] = 0.0;
    for (unsigned int i=0; i<PARTICLE_NUM; i++){
      x += this->particleCloud.poses[i].position.x/PARTICLE_NUM;
      y += this->particleCloud.poses[i].position.y/PARTICLE_NUM;
      t += tf::getYaw(this->particleCloud.poses[i].orientation)/PARTICLE_NUM;

      this->estimatedPose.pose.covariance[0] += 
        pow(this->particleCloud.poses[i].position.x - this->estimatedPose.pose.pose.position.x, 2)/PARTICLE_NUM;
      this->estimatedPose.pose.covariance[7] += 
        pow(this->particleCloud.poses[i].position.y - this->estimatedPose.pose.pose.position.y, 2)/PARTICLE_NUM;
      this->estimatedPose.pose.covariance[35] += 
        pow(tf::getYaw(this->particleCloud.poses[i].orientation) - tf::getYaw(this->estimatedPose.pose.pose.orientation), 2)/PARTICLE_NUM;
    }
    // Update sample mean
    this->estimatedPose.pose.pose.position.x = x;
    this->estimatedPose.pose.pose.position.y = y;
    this->estimatedPose.pose.pose.orientation = tf::createQuaternionMsgFromYaw(fmod(t, TWO_PI));

    ROS_INFO( "Estimate x = %f, y = %f, yaw = %f, xs = %f, ys = %f, yaws = %f", 
        this->estimatedPose.pose.pose.position.x, 
        this->estimatedPose.pose.pose.position.y, 
        tf::getYaw(this->estimatedPose.pose.pose.orientation), 
        this->estimatedPose.pose.covariance[0], 
        this->estimatedPose.pose.covariance[7], 
        this->estimatedPose.pose.covariance[35]);
    return this->estimatedPose.pose;
  }
};

#endif
