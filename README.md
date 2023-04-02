# unity-lockstep-sample
build a simple lockstep gameplay just for practice

## Basic principles

In order to get the same result with same initial info and input, everything in game logic should be deterministic. All values should be accurate and all execution should be in certain order.

### Fix point number

float is not deterministic from define, It's even impossible to assure equality of two floats. 

Use fix point number which uses a long value to simulate float. 

It will lose some precision but worthwhile.

All float-based structs should be replaced with fix-point version.

### Custom random generator

In logical scope, a random number should not be literally random.

Use a fixed seed each time and a simple generate method for same sequence of numbers every time.

### Sorted Container

Containers based on hash or similar things can't assure the order when traverse inner items.

Use sorted version of them or other custom containers to assure the identical order.

## Advanced features

### replay

It's one of the most significant advantage of lock-step.

The whole game play can be replayed using the initial info and player inputs.

This feature can also be used to detect cheating and bugs.

### snapshot

Take a snapshot of the whole gameplay, which can be used to

- check for inconsistency

  when inconsistency occurs in two clients, compare the snapshot to help locating bugs

- validate game result

  when game is finished, each client submits their final snapshot, compare these snapshots to detect cheating

- support predict and rollback

  will be illustrated in further predict section

## TODO

### optimize show effect

### predict and rollback
