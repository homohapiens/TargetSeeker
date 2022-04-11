from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.base_env import BaseEnv

build_path = "../Build/win/TargetSeeker"
env = UnityEnvironment(file_name=build_path, no_graphics=True, seed=1, side_channels=[])

env.reset()

env.close()
