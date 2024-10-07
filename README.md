The latest version of [doghappy/socket.io-client-csharp](https://github.com/doghappy/socket.io-client-csharp) causes deadlock when connecting. This is an example project to reproduce it.

To run the test SocketIO server (tested with Python 3.12):

- Navigate to the `test-sio-server` dir
- Create py venv (`python -m venv venv`)
- Active the venv (`source venv/bin/activate` or `.\venv\Scripts\Activate.ps1`)
- Install dependencies using pip (`pip install -r requirements.txt`)
- Run the server (`uvicorn main:app`)

