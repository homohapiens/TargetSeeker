import gym

from stable_baselines3 import PPO

env = gym.make('LunarLander-v2')

print(env.action_space)
print(env.observation_space)

# model = PPO('MlpPolicy', env, verbose=1)
# model.learn(total_timesteps=1000)

# obs = env.reset()
# for i in range(1000):
#     action, _state = model.predict(obs, deterministic=True)
#     obs, reward, done, info = env.step(action)
#     env.render()
#     if done:
#       obs = env.reset()