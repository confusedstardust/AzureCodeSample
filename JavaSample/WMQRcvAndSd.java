package com.example.demo.controller;

import javax.jms.JMSException;
import javax.jms.Message;
import javax.jms.MessageProducer;
import javax.jms.Session;
import javax.jms.TextMessage;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.example.demo.service.TestWMQ;


@RestController
public class TestController {
	@Autowired
	private TestWMQ testMq;
	

	@GetMapping("send")
	public String SendMessage(@RequestParam String message) throws JMSException {
		Session session=testMq.init();
		MessageProducer producer=testMq.targetQueue(session, "in queue name");
		testMq.sendMsg(session, producer, message);
		session.close();
		return "successfully sent message:"+message;
	}
	
	@GetMapping("receiveMessage")
	public String receiveMessage() throws JMSException {
		Session session=testMq.init();
		Message message=testMq.receive(session, "out queue name");
		session.close();
		String t =((TextMessage) message).getText();
		return "Message content is:"+t+"Message ID is:"+message.getJMSMessageID();
		
	}

}
