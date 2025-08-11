from flask import Flask, render_template, request, jsonify
from JsonRpcClient import JsonRpcClient

# Initialize the Flask application
app = Flask(__name__)

# Create a client for comunication to the unity server
unity_client = JsonRpcClient("127.0.0.1", 8080)

@app.route('/')
def index():
    """Serves the main HTML page."""
    return render_template('index.html')

@app.route('/update', methods=['POST'])
def update():
    """Receives slider data from the HTML page."""
    data = request.get_json()
    x_val = data.get('x')
    y_val = data.get('y')

    #Data arived    
    print(f"Received => X: {x_val}, Y: {y_val}")

    # Sending data to the unity client
    try:
        print("Calling 'MoveObject'...")
        result = unity_client.call("MoveObject", {"x": x_val, "y": y_val})
        print(f"Response: '{result}'")
    except (ConnectionError, ValueError) as e:
        print(f"\nAn error occurred: {e}")
        print("Please make sure the Unity server is running correctly.")
    
    # Send a confirmation response back (optional)
    return jsonify(status="success", x=x_val, y=y_val)

if __name__ == '__main__':
    # '0.0.0.0' makes the server accessible from any device on your local network.
    # You can change the port if 5000 is in use.
    app.run(host='0.0.0.0', port=5000, debug=True)