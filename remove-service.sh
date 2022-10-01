#!/bin/bash

remove_service(){
    echo -e "Removing $1\n"
    kubectl delete -f "$2"/deployment.yaml
}

case $1 in
  "api-gateway")
    remove_service "api-gateway" "ApiGateway"
    ;;
  "auth-service")
    remove_service "auth-service" "AuthService"
    ;;
  "bike-service")
    remove_service "bike-service" "BikeService"
    ;;
  "feedback-service")
    remove_service "feedback-service" "FeedbackService"
    ;;
  "user-service")
    remove_service "user-service" "UserService"
    ;;
  "rental-service")
    remove_service "rental-service" "RentalService"
    ;;
  "notification-service")
    remove_service "notification-service" "NotificationService"
    ;;
  "token-service")
    remove_service "token-service" "TokenService"
    ;;
  "dock-service")
    remove_service "dock-service" "DockService"
    ;;
  "eureka-server")
    remove_service "eureka-server" "eureka-server"
    ;;
  "travel-event-service")
    remove_service "travel-event-service" "TravelEventService"
    ;;
  "travel-service")
    remove_service "travel-service" "TravelService"
    ;;
  "payment-service")
    remove_service "payment-service" "PaymentService"
    ;;
  "payment-calculator-service")
    remove_service "payment-calculator-service" "PaymentCalculatorService"
    ;;
  "payment-validator-service")
    remove_service "payment-validator-service" "PaymentValidatorService"
    ;;
  "all")
    remove_service "api-gateway" "ApiGateway"
    remove_service "auth-service" "AuthService"
    remove_service "bike-service" "BikeService"
    remove_service "feedback-service" "FeedbackService"
    remove_service "user-service" "UserService"
    remove_service "rental-service" "RentalService"
    remove_service "travel-event-service" "TravelEventService"
    remove_service "travel-service" "TravelService"
    remove_service "notification-service" "NotificationService"
    remove_service "token-service" "TokenService"
    remove_service "dock-service" "DockService"
    remove_service "payment-service" "PaymentService"
    remove_service "payment-calculator-service" "PaymentCalculatorService"
    remove_service "payment-validator-service" "PaymentValidatorService"
    remove_service "eureka-server" "eureka-server"

    ;;
  *)
    echo -e "unknown \n"
    ;;
esac