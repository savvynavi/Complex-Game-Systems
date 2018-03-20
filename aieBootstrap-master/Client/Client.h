#pragma once

#include "Application.h"
#include <glm/mat4x4.hpp>
#include<RakPeerInterface.h>
#include<MessageIdentifiers.h>
#include<BitStream.h>
#include "GameMessages.h"
#include "GameObject.h"
#include"Input.h"
#include<unordered_map>

class Client : public aie::Application {
public:

	Client();
	virtual ~Client();

	virtual bool startup();
	virtual void shutdown();

	virtual void update(float deltaTime);
	virtual void draw();

	//initialize the connection
	void handleNetworkConnection();
	void initialiseClientConnection();

	//handle incomming packets
	void handleNetworkMessages();

	void onSetClientIDPacket(RakNet::Packet* packet);
	void sendClientGameObject();
	void onRecievedClientDataPacket(RakNet::Packet* packet);
protected:

	glm::mat4	m_viewMatrix;
	glm::mat4	m_projectionMatrix;
	aie::Input* m_input;

	RakNet::RakPeerInterface* m_peerInterface;

	const char* IP = "127.0.0.1";
	const unsigned short PORT = 5456;

	GameObject m_gameObject;
	int m_clientID;

	std::unordered_map<int, GameObject> m_otherClientGameObjects;
};