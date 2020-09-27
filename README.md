# Samples Repository

## ServiceBus Message Sessions

The samples included allow to send and receive session messages.

### SbMessageSessionSender

This Console application can be used to send numbered session messages to the Topic Subscription

### SbMessageSessionReceiver

This application can be used to receive session messages from a Topic Subscription

### Configuration

- Update the *ServiceBusConnectionString* in app.config
- The default Topic used is “Orders”, and the default Subscription used is “OrderTask”
- These defaults can be updated in code, if needed
