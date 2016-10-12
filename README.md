# Sample application to demonstrate .Net Core

## To run
1. Install the .Net Core SDK from [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
2. To deploy the database schema run
    ```Bash
    dotnet ef database update
    ```
3. Create a valid JWT token signed by Google. This can be done by [Using Postman](https://www.getpostman.com/docs/helpers) 
3. Start the app by running:
    ```Bash
    dotnet run
    ```
4. Hit the app and pass a header "Authorization: Bearer [Token]" in the request