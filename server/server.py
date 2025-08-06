from flask import Flask, render_template, request, jsonify

# Initialize the Flask application
app = Flask(__name__)

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
    
    # This is where the data arrives on your computer.
    # We'll just print it to the console.
    print(f"Received => X: {x_val}, Y: {y_val}")
    
    # Send a confirmation response back (optional)
    return jsonify(status="success", x=x_val, y=y_val)

if __name__ == '__main__':
    # '0.0.0.0' makes the server accessible from any device on your local network.
    # You can change the port if 5000 is in use.
    app.run(host='0.0.0.0', port=5000, debug=True)