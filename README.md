
# imusify incentify

imusify profits from active users and users are rewarded for their activity. This is the source code for the user reward NEO smart contract for the **imusify** platform that realizes this idea.

## Documentation

When a user creates an imusify account, a NEO wallet is created and assigned to him or her. The users **IMU token** balance, as well as their reputation level, is stored in the context of the smart contract on the immutable NEO blockchain. User actions on the web platform raise the users reputation level and this level, in turn, determines their IMU reward. 

In more details, events on the web user interface (see below) are signaled to the *Python* middleware designed for this project, which then triggers a return. And indeed, the awarded amount is not determined by imusify but by the contract and the users level alone. 

Beyond the NEP5 compliant method calls (which always enables users to freely exchange IMU tokens) the main chain of new methods of the imusify contract logic are as follows:

`BigInteger LevelUp(byte[] account)`

`BigInteger RewardFunction(BigInteger level)`

`BigInteger Reward(byte[] account)`

This is accompanied by the query

`BigInteger LevelOf(byte[] account)`

Before the final release, anybody can test the reward trigger and the reward scheme is designed so that the curious developer can test the functionality and quickly witness the effect of a level raise. At a later point, the scheme will be scaled for long term incentification of users.

The project is released under the MIT license.

## Web user interface

Upload music

![screen shot 2017-09-20 at 17 14 26](https://user-images.githubusercontent.com/28622235/30651921-4f77f382-9e27-11e7-8429-e0b422ae87e8.png)


Follow your favorite users

![1_imusify_home_following](https://user-images.githubusercontent.com/28622235/32715978-8db4cb96-c854-11e7-8560-270a0899f2b6.jpg)


Display song details and comments

![screen shot 2017-09-20 at 17 15 01](https://user-images.githubusercontent.com/28622235/30651920-4f74f3b2-9e27-11e7-8a85-c0e030ec82aa.png)


and purchase music for personal or commercial user

![screen shot 2017-11-13 at 09 22 58](https://user-images.githubusercontent.com/28622235/32715889-49f7a20c-c854-11e7-962e-8a5a405efd30.png)


Display most upvoted content

![3_imusify_home_playing](https://user-images.githubusercontent.com/28622235/30651574-6c8e1e84-9e26-11e7-8950-031e9a1b9fae.jpg)


and implement following functionality

![1_imusify_home_following](https://user-images.githubusercontent.com/28622235/30651606-82fe1494-9e26-11e7-90ff-a6f3e15824c0.jpg)


Create user profiles

![12_imusify_usersprofile](https://user-images.githubusercontent.com/28622235/30651598-7e55011e-9e26-11e7-9932-87aedc8e7099.jpg)


and artist profiles

![14_imusify_artistprofile_edit](https://user-images.githubusercontent.com/28622235/30651597-7dbba5e6-9e26-11e7-8025-7be154c0e6c5.jpg)


...playlists

![10_imusify_yourplaylist](https://user-images.githubusercontent.com/28622235/30651584-736ce3c0-9e26-11e7-8e3a-6221e10c60eb.jpg)


and check out this beautiful player...

![16_imusify_bigplayer](https://user-images.githubusercontent.com/28622235/30651618-8d1af690-9e26-11e7-8872-9b331b1f9f20.jpg)


Explore music by using tags and filters

![5_imusify_browse_top_explore](https://user-images.githubusercontent.com/28622235/32716000-9c12607c-c854-11e7-8fb0-37c50903aac8.jpg)
