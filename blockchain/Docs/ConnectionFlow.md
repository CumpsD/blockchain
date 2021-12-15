# Connection Flow

## Incoming Peer & Identify

* `ironfish\src\network\peerNetwork.ts`
```
this.webSocketServer.onConnection((connection, req) => {
  ...
  this.peerManager.createPeerFromInboundWebSocketConnection(connection, address)
}
```

* `ironfish\src\network\peers\peerManager.ts`
```
createPeerFromInboundWebSocketConnection(...) {
  const peer = this.getOrCreatePeer(null)
  ...
  this.initWebSocketConnection(peer, webSocket, ConnectionDirection.Inbound, hostname, port)
  
  return peer
}
```

* `ironfish\src\network\peers\peerManager.ts`
```
getOrCreatePeer(...) {
  const peer = new Peer(identity, { ... })

  // Add the peer to peers. It's new, so it shouldn't exist there already
  this.peers.push(peer)
}
```

* `ironfish\src\network\peers\connections\webSocketConnection.ts`
```
this.socket.onmessage = (event: MessageEvent) => {
  ...
  let message
  try {
    message = parseMessage(event.data)
```

* `ironfish\src\network\peers\peerManager.ts`
```
private async handleMessage(peer: Peer, connection: Connection, message: LooseMessage) {
  ...
  } else if (connection.state.type === 'WAITING_FOR_IDENTITY') {
    this.handleWaitingForIdentityMessage(peer, connection, message)
  } 
```

* Results in:
```
Connected to the Iron Fish network
Starting sync from GhoaGho (csharp-node). work: +1414803334548496, ours: 1245, theirs: 23123
Finding ancestor using linear search on last 3 blocks starting at 00000...fc0fa (1245) from peer GhoaGho (csharp-node) at 23123
```