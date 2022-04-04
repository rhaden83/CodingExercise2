Coding Exercise 2
===================

This is an updated version of my solution to the [TNTP Coding Exercise](https://github.com/tntp/Tntp.CodingExercise). The idea is to demonstrate how I work in a fashion resembling real development.

The project: "Simple Twitter", a small microblogging solution that will allow people to post a name, and a comment, and see other comments that other people have posted. Components include a database to house the comments and usernames, a web-based front end, and a web-API layer with validation logic.

## Story

```
As a user,
I would like to post comments that other people can see, and see comments that other people have posted
So that I can feel connected in this crazy disconnected world
```

## Scenarios
```
Given a user who has found the site
When that user wants to post a comment
Then that user should be able to supply their name, and a comment
```
```
Given a user who is posting a comment
When that user does not supply a name
Then they should not be able to post their comment
```
```
Given a user who is posting a comment
When that user attempts to exceed a limit of 140 characters in their comment
Then that user should not be able to post their comment
```
```
Given a user
When that user is viewing comments
Then they should see all of the comments in the system, in reverse chronological order (newer comments first)
```

## Technologies Used

- HTML5
- CSS3
- SCSS
- TypeScript
- NPM
- Angular
- C#
- .NET 5
- ASP.NET Core
- SignalR
- Entity Framework Core
- SQL Server