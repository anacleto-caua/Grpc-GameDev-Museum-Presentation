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

# ------------------------ First unity showcase showing ik ------------------------
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
    result = ik_unity_client.call("MoveObject", {"x": x_val, "y": y_val}, True)
    
    # Send a confirmation response back (optional)
    return jsonify(status="success", x=x_val, y=y_val)

# ------------------------ Second unity showcase showing procedural terrain generation ------------------------
@app.route('/procedural_showcase')
def procedural_showcase():
    return render_template('procedural_showcase.html')

@app.route('/procedural_showcase/update', methods=['POST'])
def procedural_showcase_update():
    """Receives form data and sends it to the procedural generation Unity client."""
    data = request.get_json()

    print(f"Received procedural parameters => {data}")
    
    result = procedural_unity_client.call("GenerateTerrain", data)
    return jsonify(status="success", unity_response=result)

# ------------------------ Third unity showcase showing particle effects ------------------------
@app.route('/particle_showcase')
def particle_showcase():
    """Serves the main particle showcase page."""
    return render_template('particle_showcase.html')

@app.route('/particle_showcase/update', methods=['POST'])
def particle_showcase_update():
    """Receives form data and sends it to the particle showcase Unity client."""
    data = request.get_json()

    print(f"Received particle parameters => {data}")
    
    # Assumes you have a 'particle_unity_client' instance created
    result = particle_unity_client.call("UpdateParticles", data)
    return jsonify(status="success", unity_response=result)

# ------------------------ General use ------------------------
def send_qr_code_link(client, link):
    """Sends the link so Unity can assemble the qr code."""
    result = client.call("ShowLinkQrCode", {"link": link}, True)
 
def get_local_ip():
    """Only way I found to get my IP."""
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    # Doesn't have to be reachable
    s.connect(('10.255.255.255', 1))
    IP = s.getsockname()[0]

    s.close()
    
    return IP

def make_unity_client(U_HOST, U_PORT, service_name, flask_port):
    # Create a client for comunication to the unity server
    unity_client = JsonRpcClient(U_HOST, U_PORT)

    # Send the URL to Unity before starting the server.
    local_ip = get_local_ip()
    flask_server_url = f"http://{local_ip}:{flask_port}/{service_name}"
    send_qr_code_link(unity_client, flask_server_url)

    return unity_client

if __name__ == '__main__':
    # '0.0.0.0' makes the server accessible from any device on the local network.
    HOST = "0.0.0.0"
    PORT = 5000

    global ik_unity_client, procedural_unity_client

    ik_HOST = get_local_ip()
    ik_PORT = "8080"
    ik_unity_client = make_unity_client(ik_HOST, ik_PORT, "ik_showcase", flask_port=PORT)

    pc_HOST = get_local_ip()
    pc_PORT = "8081"
    procedural_unity_client = make_unity_client(pc_HOST, pc_PORT, "procedural_showcase", flask_port=PORT)
    
    pt_HOST = get_local_ip()
    pt_PORT = "8082"
    particle_unity_client = make_unity_client(pt_HOST, pt_PORT, "particle_showcase", flask_port=PORT)

   
    # Start the Flask server
    app.run(host=HOST, port=PORT, debug=True)