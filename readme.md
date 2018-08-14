# 2 Step login verification application

It's an asp.net web application showing how to secure ASP.NET Web API using Token Based Authentication and 2 step login verification **without** using ASP.NET Identity


## Project anatomy

### Database

EmployeesDB which contains two tables; one for the system users, and one for the employees (some secured data)

**Note**: Don't save passwords as plain text in real applications, it's only in this example for simplicity.

### Web API 
A [secured service](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Controllers/SecondStepVerificationController.cs) to verify logged in users, and another [service](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Controllers/DataController.cs) to fetch employees data upon user request

### Frontend

Three html pages; a **Login** page for the 1<sup>st</sup> step, where the user got a token to be used in the 2<sup>nd</sup> step in the **Verify** page. After being verified, the user get a new token to be able to explore the site and get the employees in **Index** page

**Note**: In real applications, don't send sensitive data through HTTP Request/Response without being encrypted nor using SSL

### web.config

To define the required application and server configurations, such as database connection string

**Note**: In real applications, you should store the connection string and any sensitive data securely, as described in [Connection Strings and Configuration Files](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings-and-configuration-files)


### OWIN Start Up class



## Basic Steps to build the application

- Using Visual Studio 2015, create `Web API` `Empty` Project


    ```
   File menu > New > Projet > ASP.NET web application > enter application name & project location > OK > Empty template & Web API Checkbox ticked > OK
    ```

- Install the needed NuGet Packages responsible for implementing token based authentication:

    - Microsoft.Owin.Host.SystemWeb
    - Microsoft.Owin.Security.OAuth
    - Microsoft.Owin.Cors


    ```
    To open NuGet: Go to Solution Explorer >  Right Click on References > Manage NuGet packages > Search for these three Packages
    ```

- Add a [database](https://github.com/aelbasioni/2StepLoginVerification/tree/master/2StepVerification/App_Data) having `Users` table and `Employess` table to be used in the test

- Add a class to represent OAuth Provider for validating the user credentials and generate token (i.e [Provider/ 	ApplicationOAuthProvider.cs](https://github.com/aelbasioni/2StepLoginVerification/tree/master/2StepVerification/Providers))

  
    ```
    Go to solution explorer > Right click on the Project Name > Add > New Item > Select Class & enter its name > Add
    ```


- Add [OWIN Start Up class](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Startup.cs) to the root of the project. In this class we can configure OAuth Authorization Server, and it will be fired once our server starts

  
    ```
    Go to Solution Explorer > Right Click on Project Name > Add > New Item > Select OWIN Startup class > Enter class name > Add
    ```

- Add the client pages that represent the flow of the login process, as illustrated from the package files: [Login.html](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Login.html) > [Verify.html](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Verify.html) > [Index.html](https://github.com/aelbasioni/2StepLoginVerification/blob/master/2StepVerification/Index.html) where I used [Jquery](https://developers.google.com/speed/libraries/#jquery) for sending ajax requests and populating the pages with the coming data