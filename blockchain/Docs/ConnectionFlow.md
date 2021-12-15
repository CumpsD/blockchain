# Connection Flow

## Incoming Peer

* ironfish\src\network\peerNetwork.ts
```
this.webSocketServer.onConnection((connection, req) => {
  ...
  this.peerManager.createPeerFromInboundWebSocketConnection(connection, address)
}
```

* ironfish\src\network\peers\peerManager.ts
```
createPeerFromInboundWebSocketConnection(...) {
  const peer = this.getOrCreatePeer(null)
  ...
  this.initWebSocketConnection(peer, webSocket, ConnectionDirection.Inbound, hostname, port)
  
  return peer
}
```

* ironfish\src\network\peers\peerManager.ts
```
getOrCreatePeer(...) {
  const peer = new Peer(identity, { ... })

  // Add the peer to peers. It's new, so it shouldn't exist there already
  this.peers.push(peer)
}
```
