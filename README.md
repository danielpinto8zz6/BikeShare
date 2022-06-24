
| Service | Port |
|--|--|
| api-gateway  | 8099 |
| auth-service | 5010 |
| bike-service | 5020 |
| feedback-service | 5030 |
| user-service | 5040 |
| rental-service | 5050 |
| travel-service | 5060 |
| rental-process-service | 5070 |
| notification-service | 5080 |
| token-service | 5090 |
| dummy-dock-service | 6000 |
| dock-service | 6010 |
| bike-validate-service | 6020 |
| payment-service | 6030 |
| payment-calculator-service | 6040 |
| travel-event-service | 6050 |

db.dock.createIndex( { geometry : "2dsphere" } )