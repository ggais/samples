# ServiceBus Message Sessions

For scalable applications, the application components are decoupled to scale independently. These application components can use asynchronous messaging for interaction. The general recommendation is to keep these application components stateless, so the message ordering does not matter, and can be processed in parallel. 
But there can be scenarios, where the messages need to be processed in order, they were received using FIFO (First-in, first out) pattern. Service Bus supports ordering guarantee using messaging sessions, and can be used for the FIFO pattern.

## Setup

### Azure Service Bus Setup

1. Go to your Service Bus instance in Azure
2. Create a Topic named "Orders"
3. Create a Subscription named "OrderTask"
    * Remove the existing filter
    * Add a SQL filter "OrderTaskFilter" with this expression - SubscriberName = 'OrderTask'
4. Copy the ServiceBus ConnectionString
    * The ConnectionString should be for the ServiceBus namespace with Send and Listen permissions

### Configuration

1. Update the *ServiceBusConnectionString* in App.config for both SbMessageSessionSender and SbMessageSessionReceiver applications

## Applications

The samples include 2 applications

### SbMessageSessionSender Application

This Console application can be used to send numbered session messages to the Topic Subscription

1. Run the Console application
2. First prompt - "Enter the total count of messages to log:"
    * The application will generate total messages based on this count, starting with Index 1.
3. Next prompt - "Enter delay between messages in milliseconds: "
    * Enter the desired delay between any messages
    * Enter 0 for no delay
4. Next prompt - "Press any key to start processing"
5. The messages are generated and sent to the ServiceBus Topic, starting with the index 1, and incrementing until the total count of messages is reached. The SessionId is auto-generated.
6. Once all the messages are sent, it prompts "Do you want to re-run (y/n)?"
    * To resend the messages press "y". This will resend the messages with new SessionId
    * To exit press "n"

### SbMessageSessionReceiver Application

This application can be used to receive session messages from a Topic Subscription. 

1. Run multiple instances of this application, by double-clicking the SbMessageSessionReceiver.exe
2. You should notice that, all the messages received for a SessionId, will be processed by the same instance.