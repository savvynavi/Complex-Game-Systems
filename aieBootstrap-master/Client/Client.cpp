#include "Client.h"
#include "Gizmos.h"
#include "Input.h"
#include <glm/glm.hpp>
#include <glm/ext.hpp>
#include <iostream>



using glm::vec3;
using glm::vec4;
using glm::mat4;
using aie::Gizmos;

Client::Client() {

}

Client::~Client() {
}

bool Client::startup() {
	
	setBackgroundColour(0.25f, 0.25f, 0.25f);

	// initialise gizmo primitive counts
	Gizmos::create(10000, 10000, 10000, 10000);

	// create simple camera transforms
	m_viewMatrix = glm::lookAt(vec3(10), vec3(0), vec3(0, 1, 0));
	m_projectionMatrix = glm::perspective(glm::pi<float>() * 0.25f,
										  getWindowWidth() / (float)getWindowHeight(),
										  0.1f, 1000.f);

	handleNetworkConnection();

	m_input = aie::Input::getInstance();

	m_gameObject.position = glm::vec3(0, 0, 0);
	m_gameObject.colour = glm::vec4(1, 0, 0, 1);
	return true;
}

void Client::shutdown() {

	Gizmos::destroy();
}

void Client::update(float deltaTime) {

	// query time since application started

	float time = getTime();
	handleNetworkMessages();
	// wipe the gizmos clean for this frame
	Gizmos::clear();

	//simple movement code
	if(m_input->isKeyDown(aie::INPUT_KEY_LEFT)){
		m_gameObject.position.x -= 10.0f * deltaTime;
		sendClientGameObject();
	}
	if(m_input->isKeyDown(aie::INPUT_KEY_RIGHT)){
		m_gameObject.position.x += 10.0f * deltaTime;
		sendClientGameObject();
	}
	
	// quit if we press escape
	//aie::Input* input = aie::Input::getInstance();

	if (m_input->isKeyDown(aie::INPUT_KEY_ESCAPE))
		quit();
}

void Client::draw() {

	// wipe the screen to the background colour
	clearScreen();

	Gizmos::addSphere(m_gameObject.position, 1.0f, 32, 32, m_gameObject.colour);
	// update perspective in case window resized
	m_projectionMatrix = glm::perspective(glm::pi<float>() * 0.25f,
										  getWindowWidth() / (float)getWindowHeight(),
										  0.1f, 1000.f);

	Gizmos::draw(m_projectionMatrix * m_viewMatrix);

	
}

void Client::handleNetworkConnection(){
	//init the raknet peer interface first
	m_peerInterface = RakNet::RakPeerInterface::GetInstance();
	initialiseClientConnection();
}

void Client::initialiseClientConnection(){
	//create socket descriptor to describe this connection, no data needed as we will be connecting to a server
	RakNet::SocketDescriptor sd;

	//now call startup - max of 1 connections (to the server)
	m_peerInterface->Startup(1, &sd, 1);

	std::cout << "Connecting to server at: " << IP << std::endl;

	//now call connnect to attempt to connect to the given server
	RakNet::ConnectionAttemptResult result = m_peerInterface->Connect(IP, PORT, nullptr, 0);

	//finallly check to see if we connected, if not throw error
	if(result != RakNet::CONNECTION_ATTEMPT_STARTED){
		std::cout << "Unable to start connection, Error number: " << result << std::endl;
	}
}

void Client::handleNetworkMessages(){
	RakNet::Packet* packet;
	for(packet = m_peerInterface->Receive(); packet; m_peerInterface->DeallocatePacket(packet), packet = m_peerInterface->Receive()){
		switch(packet->data[0]){
		case ID_REMOTE_DISCONNECTION_NOTIFICATION:
			std::cout << "Another client has disconnected.\n";
			break;
		case ID_REMOTE_CONNECTION_LOST:
			std::cout << "Another client has lost the connection.\n";
			break;
		case ID_REMOTE_NEW_INCOMING_CONNECTION:
			std::cout << "Another client has connected.\n";
			break;
		case ID_CONNECTION_REQUEST_ACCEPTED:
			std::cout << "Our connection request has been accepted.\n";
			break;
		case ID_NO_FREE_INCOMING_CONNECTIONS:
			std::cout << "The server is full.\n";
			break;
		case ID_DISCONNECTION_NOTIFICATION:
			std::cout << "We have been disconnected.\n";
			break;
		case ID_CONNECTION_LOST:
			std::cout << "Connection lost.\n";
			break;
		case ID_SERVER_TEXT_MESSAGE:
		{
			RakNet::BitStream bsIn(packet->data, packet->length, false);
			bsIn.IgnoreBytes(sizeof(RakNet::MessageID));

			RakNet::RakString str;
			bsIn.Read(str);
			std::cout << str.C_String() << std::endl;
			break;
		}
		case ID_SERVER_SET_CLIENT_ID:
			onSetClientIDPacket(packet);
			break;
		case ID_CLIENT_CLIENT_DATA:
			onRecievedClientDataPacket(packet);
			break;
		default:
			std::cout << "Recieved unknown message id: " << packet->data[0];
			break;
		}
	}
}

void Client::onSetClientIDPacket(RakNet::Packet* packet){
	RakNet::BitStream bsIn(packet->data, packet->length, false);
	bsIn.IgnoreBytes(sizeof(RakNet::MessageID));
	bsIn.Read(m_clientID);

	std::cout << "Set my client ID to: " << m_clientID << std::endl;
}

void Client::sendClientGameObject(){
	RakNet::BitStream bs;
	bs.Write((RakNet::MessageID)GameMessages::ID_CLIENT_CLIENT_DATA);
	bs.Write(m_clientID);
	bs.Write((char*)&m_gameObject, sizeof(GameObject));

	m_peerInterface->Send(&bs, HIGH_PRIORITY, RELIABLE_ORDERED, 0, RakNet::UNASSIGNED_SYSTEM_ADDRESS, true);
}

void Client::onRecievedClientDataPacket(RakNet::Packet* packet){
	RakNet::BitStream bsIn(packet->data, packet->length, false);
	bsIn.IgnoreBytes(sizeof(RakNet::MessageID));

	int clientID;
	bsIn.Read(clientID);

	//if clientID doesn't match our id, we neeed to update tour clinet gameObject info
	if(clientID != m_clientID){
		GameObject clientData;
		bsIn.Read((char*)&clientData, sizeof(GameObject));

		//just outputting gameobj info to console, change later
		std::cout << "Client " << clientID << " at: " << clientData.position.x << " " << clientData.position.z << std::endl;
	}
}