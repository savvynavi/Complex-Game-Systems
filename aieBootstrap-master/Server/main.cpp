#include<iostream>
#include<string>
#include<thread>
#include<chrono>

#include<RakPeerInterface.h>
#include<MessageIdentifiers.h>
#include<BitStream.h>
#include"GameMessages.h"

void handleNetworkMessages(RakNet::RakPeerInterface* peerInterface, int& nextClientID);
void sendClientPing(RakNet::RakPeerInterface* peerInterface);
void sendNewClientID(RakNet::RakPeerInterface* peerInterface, RakNet::SystemAddress& address, int& nextClientID);

int main()
{
	const unsigned short PORT = 5456;
	RakNet::RakPeerInterface* peerInterface = nullptr;

	//startup the server and start it listening to clients
	std::cout << "Starting up the server..." << std::endl;

	//initialize the RakNet peer interface first
	peerInterface = RakNet::RakPeerInterface::GetInstance();

	//create a socket descriptor to describe tis connection
	RakNet::SocketDescriptor sd(PORT, 0);

	//now call startup - max of 32 connections on the assigned port
	peerInterface->Startup(32, &sd, 1);
	peerInterface->SetMaximumIncomingConnections(32);

	//client ID stuff
	int nextClientID = 1;


	std::thread pingThread(sendClientPing, peerInterface);
	//infinite loop, don't put anything beyond this point
	handleNetworkMessages(peerInterface, nextClientID);

	return 0;
}

void handleNetworkMessages(RakNet::RakPeerInterface* peerInterface, int& nextClientID){
	RakNet::Packet* packet = nullptr;
	while(true){
		for(packet = peerInterface->Receive(); packet; peerInterface->DeallocatePacket(packet), packet = peerInterface->Receive()){
			switch(packet->data[0]){
			case ID_NEW_INCOMING_CONNECTION:
				std::cout << "A connection  is incoming.\n";
				sendNewClientID(peerInterface, packet->systemAddress, nextClientID);
				break;
			case ID_DISCONNECTION_NOTIFICATION:
				std::cout << "A client has disconncted.\n";
				break;
			case ID_CONNECTION_LOST:
				std::cout << "A client lost the connection.\n";
				break;
			case ID_CLIENT_CLIENT_DATA:
			{
				RakNet::BitStream bs(packet->data, packet->length, false);
				peerInterface->Send(&bs, HIGH_PRIORITY, RELIABLE_ORDERED, 0, packet->systemAddress, true);
				break;
			}
			default:
				std::cout << "Recieved unknown message id: " << (int)packet->data[0];
				break;
			}
		}
	}
}

void sendClientPing(RakNet::RakPeerInterface* peerInterface){
	while(true){
		RakNet::BitStream bs;
		bs.Write((RakNet::MessageID)GameMessages::ID_SERVER_TEXT_MESSAGE);
		bs.Write("Ping!");

		peerInterface->Send(&bs, HIGH_PRIORITY, RELIABLE_ORDERED, 0, RakNet::UNASSIGNED_SYSTEM_ADDRESS, true);
		std::this_thread::sleep_for(std::chrono::seconds(1));
	}
}

void sendNewClientID(RakNet::RakPeerInterface* peerInterface, RakNet::SystemAddress& address, int& nextClientID){
	RakNet::BitStream bs;
	bs.Write((RakNet::MessageID)GameMessages::ID_SERVER_SET_CLIENT_ID);
	bs.Write(nextClientID);
	nextClientID++;

	peerInterface->Send(&bs, HIGH_PRIORITY, RELIABLE_ORDERED, 0, address, false);
}