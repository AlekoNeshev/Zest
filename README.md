# Zest 
## Твоят път към науката

API е разработен на .NET Web API, написан на езика за програмиране C#. Дадената технология е избрана, зареди нейната надеждност, добра и подробна документация и голям избор от публични ресурси.

[Documentation for Zest](https://docs.google.com/document/d/1_bVuJu_zScgK3iwTjZTo0jCmbtr0PDWtfIlekuLMpU8/edit?usp=sharing)

## Дизайн

- API е състван от 5 проекта:
-- Zest - Web Application
-- Zest_DBModels - Библиотека, в която стои DBContext и DB моделите
-- Zest_Services  - Библиотека, в което стоят услугите на API. 
-- Zest_ViewModels - Библиотека, в която стоят DTO на DB моделите
-- Zest_UnitTests - Тестове за API

## Services
- AccountService
-- Предоставя всички нужни услуги за управление на потребителите в социалната мрежа.
- CommentService 
-- Предоставя всички нужни услуги за управление на коментарите в социалната мрежа.
- CommunityFollowerService
-- Предоставя всички нужни услуги за управление на последователите на общности в социалната мрежа.
- CommunityService
-- Предоставя всички нужни услуги за управление на общностите в социалната мрежа.
- FollowerService
-- Предоставя всички нужни услуги за управление на следванията между потребители в социалната мрежа.
- LikeService 
-- Предоставя всички нужни услуги за управление на реакциите на потребителите в социалната мрежа. Реакциите могат да бъдат сложени на публикация или коментар.
- MessageService
-- Предоставя всички нужни услуги за управление на съобщенията между потребителите в социалната мрежа.
- PostResourcesService
-- Предоставя всички нужни услуги за управление на ресурсите (снимки или видео) към дадена публикация в социалната мрежа.
- PostService
-- Предоставя всички нужни услуги за управление на публикациите в социалната мрежа.
- SignalRService 
-- Предоставя всички нужни услуги за управление на SignalR хъбовете в социалната мрежа, като добавя или премахва потребител с даден идентификатор на връзката от SignalR група.
## Котролери
- AccountController
-Endpoints:
```sh
get
add/{name}/{email}
getAll/{takeCount}/{skipCount}
getBySearch/{search}/{takeCount}
```
- CommentsController
-Endpoints:
```sh
{id}
add/post/{postId}/comment/{commentId}
remove/{commentId}/{postId}
getCommentsByPost/{postId}/{lastDate}/{takeCount}
getByTrending/{takeCount}/{postId}
```
- LikesController
-Endpoints:
```sh
add/post/{postId}/comment/{commentId}/value/{value}
remove/like/{likeId}/{postId}/{commentId}
```
- CommunityController
-Endpoints:
```sh
{id}
getAll/{takeCount}/{skipCount}
add/{name}
delete/{communityId}
getByAccountId/{accountId}/{takeCount}/{skipCount}
getByPopularityId/{takeCount}
getBySearch/{search}/{takeCount}
```
- CommunityFollowersController
-Endpoints:
```sh
account/add/community/{communityId}
account/delete/community/{communityId}
```
- CommunityModeratorsController
-Endponts:
```sh
isModerator/{accountId}/{communityId}
add/{accountId}/{communityId}
getModerators/{communityId}
getCandidates/{communityId}
approveCandidate/{accountId}/{communityId}
removeModerator/{accountId}/{communityId}
```
- FollowersController
-Endpoints:
```sh
followers/find/{followerId}/{followedId}
add/followed/{followedId}
delete/followed/{followedId}
getFriends/{takeCount}/{skipCount}
getBySearch/{search}/{takeCount}
```
- MessagesController
-Endpoints:
```sh
get/{id}
get/receiver/{receiverId}/{takeCount}/{date}
add/receiver/{receiverId}
```
- PostController
-Endpoints:
```sh
{id}
add/{title}/community/{communityId}
remove/{postId}
getByDate/{lastDate}/{communityId}/{takeCount}
getByCommunity/{communityId}
getBySearch/{search}/{takeCount}/{communityId}
getByTrending/{takeCount}/{communityId}
getByFollowed/{takeCount}
```
- PostResourcesController
-Endpoints:
```sh
uploadFile/{postId}
get/{fileName}
getByPostId/{postId}
```
- SignalRGroupsController
-Endpoints:
```sh
addConnectionToGroup/{connectionId}
removeConnectionToGroup/{connectionId
```
