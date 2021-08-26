# Reuseventory API
This is API to be used with the Angular Special Topics course at Ithaca college. 

## Building and Running in a Docker Container

This section describes how you can run the API in a docker container. In the examples below, the image name will be `resuventory` and the container name will be `reuseventory-api`. 

```
docker build -t reuseventory .
docker run -d -p 8080:80 --name reuseventory-api reuseventory
```

You should then be able to access the API at `http://localhost:8080/api/listings`


## Releasing and Deploying to Heroku

This section describes how to deploy the application and make it available to the world wide web. Prior to running these steps, be sure to have logged into Heroku and created an app with the name `reuseventory`.

1. Log into Heroku
```
heroku container:login
```

2. If you haven't already, build the container
```
docker build -t reuseventory .
```

3. Push the container to Heroku
```
heroku container:push -a reuseventory web
```

4. Release it
```
heroku container:release -a reuseventory web
```