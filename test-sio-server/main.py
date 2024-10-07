import socketio

# create a Socket.IO server
sio = socketio.AsyncServer(async_mode='asgi')

# wrap with ASGI application
app = socketio.ASGIApp(sio)


@sio.event
def connect(sid, environ, auth):
    print('connect ', sid)


@sio.event
def disconnect(sid):
    print('disconnect ', sid)


@sio.on('foo')
def echo_foo_event(sid, data):
    sio.emit('foo', {"data": "bar", "received": data}, to=sid)
