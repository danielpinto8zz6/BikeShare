FROM openjdk:11-jre-slim-buster
VOLUME /tmp
ADD build/libs/eureka-server-0.0.1-SNAPSHOT.jar app.jar
ENTRYPOINT ["java","-jar","app.jar"]
EXPOSE 8761