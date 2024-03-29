import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Hashtable;

import javax.jms.*;

import org.springframework.context.annotation.Configuration;

import com.ibm.msg.client.jms.*;
import com.ibm.msg.client.wmq.*;
@Configuration
public class TestWMQ
{
   private static final SimpleDateFormat  LOGGER_TIMESTAMP = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss.SSS");
   private Hashtable<String,String> params;

   
   
   
   public Session  init() 
   {
      Connection conn = null;
      Session session = null;
//      Destination destination = null;
//      MessageProducer producer = null;
      
      try
      {
         //System.setProperty("com.ibm.mq.cfg.useIBMCipherMappings", "false");
         JmsFactoryFactory ff = JmsFactoryFactory.getInstance(WMQConstants.WMQ_PROVIDER);
         JmsConnectionFactory cf = ff.createConnectionFactory();
         // Set the properties
         cf.setStringProperty(WMQConstants.WMQ_HOST_NAME, "Host Name");
         cf.setIntProperty(WMQConstants.WMQ_PORT, 1414);
         cf.setStringProperty(WMQConstants.WMQ_CHANNEL, "Channel Name");
         cf.setStringProperty(WMQConstants.WMQ_QUEUE_MANAGER, "QueueManagerName");
         cf.setStringProperty(WMQConstants.WMQ_SSL_CIPHER_SUITE, "TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384");
         cf.setStringProperty(WMQConstants.USERID,"ServiceAccount");
         cf.setStringProperty(WMQConstants.PASSWORD,"your password");
         cf.setIntProperty(WMQConstants.WMQ_CONNECTION_MODE, WMQConstants.WMQ_CM_CLIENT);
         //cf.setBooleanProperty(WMQConstants.USER_AUTHENTICATION_MQCSP, true);
         //cf.setStringProperty(WMQConstants.WMQ_SSL_KEY_REPOSITORY, null);       
         conn = cf.createConnection();
         conn.start();
         session = conn.createSession(false, Session.AUTO_ACKNOWLEDGE);
      }
      catch (JMSException e)
      {
         if (e != null)
         {
            logger(e.getLocalizedMessage());
            e.printStackTrace();
            
            Exception gle = e.getLinkedException();
            if (gle != null)
               logger(gle.getLocalizedMessage());
         }
      }
      return session;
   }

   public MessageProducer targetQueue(Session session,String targetqueue) {
	   Destination destination=null;
	   MessageProducer producer=null;
	try {
		destination = session.createQueue(targetqueue);
	} catch (JMSException e1) {
		// TODO Auto-generated catch block
		e1.printStackTrace();
	}
	try {
		producer = session.createProducer(destination);
	} catch (JMSException e) {
		e.printStackTrace();
	}
    
	   return producer;
	
}
   /**
    * Send a message to a queue.
    */
   public void sendMsg(Session session, MessageProducer producer,String testmsg)
   {
      try
      {
         TextMessage msg = session.createTextMessage("test message from Eden "+testmsg);

         producer.send(msg);
         logger("Sent message: " + msg);
      }
      catch (JMSException e)
      {
         if (e != null)
         {
            logger(e.getLocalizedMessage());
            e.printStackTrace();
            
            Exception gle = e.getLinkedException();
            if (gle != null)
               logger(gle.getLocalizedMessage());
         }
      }
   }

   /**
    * A simple logger method
    * @param data
    */
   public static void logger(String data)
   {
      String className = Thread.currentThread().getStackTrace()[2].getClassName();

      // Remove the package info.
      if ( (className != null) && (className.lastIndexOf('.') != -1) )
         className = className.substring(className.lastIndexOf('.')+1);

      System.out.println(LOGGER_TIMESTAMP.format(new Date())+" "+className+": "+Thread.currentThread().getStackTrace()[2].getMethodName()+": "+data);
   }


   public Message receive(Session session,String targetQueue)
   {
	   MessageConsumer consumer=null;
	try {
		
		Destination destination = session.createQueue(targetQueue);
		consumer=session.createConsumer(destination);
		return consumer.receive();
		
	} catch (JMSException e) {
		// TODO Auto-generated catch block
		e.printStackTrace();
	}
	return null;
   }
}
