# Samples Repository

## ServiceBus Message Sessions

For scalable applications, the application components are decoupled to scale independently. These application components can use asynchronous messaging for interaction. The general recommendation is to keep these application components stateless, so the message ordering does not matter, and can be processed in parallel. 
But there can be scenarios, where the messages need to be processed in order, they were received using FIFO (First-in, first out) pattern. Service Bus supports ordering guarantee using messaging sessions, and can be used for the FIFO pattern.

The samples included allow to send and receive session messages.

### SbMessageSessionSender

This Console application can be used to send numbered session messages to the Topic Subscription

### SbMessageSessionReceiver

This application can be used to receive session messages from a Topic Subscription. The messages are written to a file in *Processed* folder in application directory. The autogenerated *AgentId Guid* is used as the filename. Multiple instances of this application can be run.

### Configuration

- Update the *ServiceBusConnectionString* in app.config
- The default Topic used is “Orders”, and the default Subscription used is “OrderTask”
- These defaults can be updated in code, if needed
