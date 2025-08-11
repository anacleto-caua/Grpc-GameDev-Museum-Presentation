from flask import Flask, render_template, request, jsonify
from JsonRpcClient import JsonRpcClient
import socket # Just to get the ip address

# Initialize the Flask application
app = Flask(__name__)

# index landing page.
@app.route('/')
def index():
    """Serves the main HTML page."""
    return render_template('index.html')

@app.route('/ik_showcase')
def ik_showcase():
    """Serves the main ik showcase page."""
    return render_template('ik_showcase.html')

@app.route('/ik_showcase/update', methods=['POST'])
def ik_showcase_update():
    """Receives slider data from the HTML page."""
    data = request.get_json()
    x_val = data.get('x')
    y_val = data.get('y')

    # Data arived    
    print(f"Received => X: {x_val}, Y: {y_val}")
    result = unity_client.call("MoveObject", {"x": x_val, "y": y_val}, True)
    
    # Send a confirmation response back (optional)
    return jsonify(status="success", x=x_val, y=y_val)

def send_qr_code_link(link):
    """Sends the link so Unity can assemble the qr code."""
    result = unity_client.call("ShowLinkQrCode", {"link": link}, True)

def get_local_ip():
    """Only way I found to get my IP."""
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    try:
        # Doesn't have to be reachable
        s.connect(('10.255.255.255', 1))
        IP = s.getsockname()[0]
    except Exception:
        IP = '127.0.0.1'
        print("IP not found, using local!!!")
    finally:
        s.close()
    
    return IP

if __name__ == '__main__':
    
    # Create a client for comunication to the unity server
    U_HOST = "127.0.0.1"
    U_PORT = "8080"
    unity_client = JsonRpcClient(U_HOST, U_PORT)


    # OPENING THE FLASK SERVER!!!!
    # '0.0.0.0' makes the server accessible from any device on the local network.
    HOST = "0.0.0.0"
    PORT = 5000

    # Send the URL to Unity before starting the server.
    local_ip = get_local_ip()
    flask_server_url = f"http://{local_ip}:{PORT}/ik_showcase"
    send_qr_code_link(flask_server_url)

    # Start the Flask server
    app.run(host=HOST, port=PORT, debug=True)