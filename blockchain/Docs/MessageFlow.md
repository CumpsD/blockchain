# Message Flow

## Identify

Somebody (`identity`) identifies with us:

```
{
  type: 'identity',
  payload: {
    identity: 'bwBpkJX5VAtFOt2K/DFuJ+KrlkWy2YNDkxe19TKnlCI=',
    version: 11,
    agent: 'if/cli/4b3079f',
    name: 'bootstrap',
    port: 9033,
    head: '8fb5b576eb519e03911c8768d7c8ac97252f7be49fec77279c43af2da9f82068',
    work: '131072',
    sequence: 1
  }
}
```

```
{
  type: 'identity',
  payload: {
    identity: 'I3TRELuDr00NLnLZquB8plLsvdSWiP7cHJo7Ym/Va1U=',
    version: 11,
    agent: 'if/cli/src',
    name: 'Delonge',
    port: 9033,
    head: '0000000017996e3b061fb7118db7007084000a28306a285f193b2551854343bd',
    work: '1414804792318651',
    sequence: 23123
  }
}
```

```
{
  type: 'identity',
  payload: {
    identity: 'QnP63vs44u8/exGp576b40f8tPag1thX+RKS1BH06kg=',
    version: 11,
    agent: 'if/cli/191d772',
    name: 'gogolunpaltosu',
    port: 9033,
    head: '000000001487f03ed5370b65964079134a348a933120cd4addc83adbcaebabe0',
    work: '1253988239516873',
    sequence: 16562
  }
}
```

## Signal

Somebody (`sourceIdentity`) is sending us (`destinationIdentity`) a signal.

```
{
  type: 'signal',
  payload: {
    sourceIdentity: 'bwBpkJX5VAtFOt2K/DFuJ+KrlkWy2YNDkxe19TKnlCI=',
    destinationIdentity: 'yM9Gk00zKfXuoD0u+f9xUyi5gmBSSQjzkg+15HTXSUg=',
    nonce: 'hUnhyV15u1Iwe8DVoTSckgrWQkWJAv65',
    signal: 'L+cVgVqX9KfB5vMZw6BKrV90IQC+DAmBBrZS6Bgt8YyVcHLZ2mJO5iXKZIvkNBMWmjXCQ3+JLRiHJ2Ekaz5UWyYSQva59Bf+IX1geedoZja0Cvklb2dm14HI0Gs4eGGgazo0Jp7aLemajfYmUri5cWuglpselVXHQXC1IBnuzzJCHwtg50O28RR6inC5H1IwO0f0py9FWVM='
  }
}
```

```
{
  type: 'signal',
  payload: {
    sourceIdentity: 'Yp4iOxTltW2H+Fd3LeQoK94zj5hjsMoW0rg8GOhatF0=',
    destinationIdentity: 'yM9Gk00zKfXuoD0u+f9xUyi5gmBSSQjzkg+15HTXSUg=',
    nonce: 'HkRFpIybBP3uHXMizOkcPOltA9i8RSNp',
    signal: '08/i8OjLoB5exDoleXkuDreT7g7+Q71LZ9fGds/DVU99nK5BWHtO9LZsEJc/ZaPu2JYxKLvKMce5BDYrQ42rG/WPpz9umvNt//pC1HqKcoJbynhc2WaYeJTcYUEv+p5Hp/O3TGPBJEKBpkr7D9OvQVf33IIKv2M9pLhNHUBn6uLzCcNQ94+WSKB1QeTXba6gWp1oNdhd5lAwWt0cun2SYaIjinNnzQiiZnUmAMCEAl7EymI='
  }
}
```

## Signal Request

Somebody (`sourceIdentity`) is asking us (`destinationIdentity`) for a signal.

```
{
  type: 'signalRequest',
  payload: {
    sourceIdentity: 'UYcnr0B2l2nemK3FxCybdsgpcKliLwuNwoJ0+5vruXk=',
    destinationIdentity: 'yM9Gk00zKfXuoD0u+f9xUyi5gmBSSQjzkg+15HTXSUg='
  }
}
```

## Peer List

```
{
  type: 'peerList',
  payload: {
    connectedPeers: [
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object], [Object], [Object],
      [Object], [Object], [Object], [Object],
      ... 61 more items
    ]
  }
}
```

## Peer List Request

```
{ type: 'peerListRequest' }
```

## GetBlockHashes

```
{
  type: 'GetBlockHashes',
  rpcId: 1,
  direction: 'response',
  payload: {
    blocks: [
      '00000ee92ca9e6721d1eedc08e470f198b10b1a9e1fd3ad5cc1d247eed3ddc0e'
    ]
  }
}
```

## Disconnecting

Somebody (`sourceIdentity`) is disconnecting from us (`destinationIdentity`).

```
{
  type: 'disconnecting',
  payload: {
    sourceIdentity: 'Oj5GlcvvDMPHj670uKrghSFCW+D/KuAlby8CMlb9xDs=',
    destinationIdentity: 'yM9Gk00zKfXuoD0u+f9xUyi5gmBSSQjzkg+15HTXSUg=',
    reason: 1,
    disconnectUntil: 1639575220159
  }
}
```