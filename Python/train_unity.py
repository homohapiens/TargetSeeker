import numpy as np
import os

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel

from gym_unity.envs import UnityToGymWrapper

from stable_baselines3 import PPO
from stable_baselines3.common.evaluation import evaluate_policy
from stable_baselines3.common.monitor import Monitor


# Select Mode
# 0:Train, 1:Test, 2:Debug
mode = 1
no_graphics = False if mode == 1 else True
build_path = '../Build/win/TargetSeeker'
run_id = 'ppo_targetseeker_run'
model_save_dir = f'results/'

channel = EngineConfigurationChannel()
channel.set_configuration_parameters(width=1000, height=1000, time_scale=20)
unity_env = UnityEnvironment(file_name=build_path, no_graphics=no_graphics, seed=1, side_channels=[channel])

log_dir = "tmp/"
os.makedirs(log_dir, exist_ok=True)

env = UnityToGymWrapper(unity_env, uint8_visual=False, flatten_branched=False, allow_multiple_obs=False)
env = Monitor(env, log_dir)


if mode == 0:
    model = PPO('MlpPolicy', env, verbose=0, tensorboard_log="results/")
    model.learn(total_timesteps=1_00_000, tb_log_name=run_id)
    model.save("TargetSeeker")
    del model

elif mode == 1:
    model = PPO.load("TargetSeeker", env)
    mean_reward, std_reward = evaluate_policy(model, model.get_env(), n_eval_episodes=10)
    print(f'Mean Reward {mean_reward}')
    
    obs = env.reset()
    for i in range(1000):
        action, _state = model.predict(obs, deterministic=True)
        obs, reward, done, info = env.step(action)
        env.render()
        if done:
            obs = env.reset()

elif mode == 2:
    print('Action Space:', env.action_space)
    print('Observation Space:', env.observation_space)

env.close()