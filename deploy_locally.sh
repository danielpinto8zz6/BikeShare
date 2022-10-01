#!/bin/bash

deploy_service(){
    echo -e "Building $1 docker image... \n"
    docker build -t "$1" -f "$2"/Dockerfile .
    echo -e "Saving $1 container as tar... \n"
    docker save --output docker-images/"$1".tar "$1":latest
    sudo k3s ctr images import docker-images/$1.tar
    echo -e "Reloading $1 in k3s \n"
    kubectl delete -f "$2"/deployment.yaml
    kubectl apply -f "$2"/deployment.yaml
    echo -e "$1 deployed. \n"
}

case $1 in
  "api-gateway")
    deploy_service "api-gateway" "ApiGateway"
    ;;
  "auth-service")
    deploy_service "auth-service" "AuthService"
    ;;
  "bike-service")
    deploy_service "bike-service" "BikeService"
    ;;
  "feedback-service")
    deploy_service "feedback-service" "FeedbackService"
    ;;
  "user-service")
    deploy_service "user-service" "UserService"
    ;;
  "rental-service")
    deploy_service "rental-service" "RentalService"
    ;;
  "notification-service")
    deploy_service "notification-service" "NotificationService"
    ;;
  "token-service")
    deploy_service "token-service" "TokenService"
    ;;
  "dock-service")
    deploy_service "dock-service" "DockService"
    ;;
  "eureka-server")
    deploy_service "eureka-server" "eureka-server"
    ;;
  "travel-event-service")
    deploy_service "travel-event-service" "TravelEventService"
    ;;
  "travel-service")
    deploy_service "travel-service" "TravelService"
    ;;
  "payment-service")
    deploy_service "payment-service" "PaymentService"
    ;;
  "payment-calculator-service")
    deploy_service "payment-calculator-service" "PaymentCalculatorService"
    ;;
  "payment-validator-service")
    deploy_service "payment-validator-service" "PaymentValidatorService"
    ;;
  "all")
    deploy_service "eureka-server" "eureka-server"
    deploy_service "api-gateway" "ApiGateway"
    deploy_service "auth-service" "AuthService"
    deploy_service "bike-service" "BikeService"
    deploy_service "feedback-service" "FeedbackService"
    deploy_service "user-service" "UserService"
    deploy_service "rental-service" "RentalService"
    deploy_service "travel-event-service" "TravelEventService"
    deploy_service "travel-service" "TravelService"
    deploy_service "notification-service" "NotificationService"
    deploy_service "token-service" "TokenService"
    deploy_service "dock-service" "DockService"
    deploy_service "payment-service" "PaymentService"
    deploy_service "payment-calculator-service" "PaymentCalculatorService"
    deploy_service "payment-validator-service" "PaymentValidatorService"
    ;;
  *)
    echo -e "unknown \n"
    ;;
esac