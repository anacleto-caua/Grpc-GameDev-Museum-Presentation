import requests
import json

class JsonRpcClient:
    """A simple and reusable JSON-RPC client for making remote procedure calls."""
    
    def __init__(self, ip, port):
        """
        Initializes the client for a specific server.

        :param server_url: The URL of the JSON-RPC server (e.g., "http://localhost:8080").
        """
        self.server_url = f"http://{ip}:{port}"
        self.request_id = 0
        self.headers = {"Content-Type": "application/json"}

    def call(self, method: str, params: dict = None):
        """
        Calls a remote method on the JSON-RPC server.

        :param method: The name of the method to call.
        :param params: A dictionary of parameters for the method.
        :return: The 'result' from the server's response.
        :raises ConnectionError: If there is a network issue or a server-level error.
        :raises ValueError: If the response is not valid JSON or is a JSON-RPC error.
        """
        self.request_id += 1
        
        payload = {
            "method": method,
            "params": params or {},
            "jsonrpc": "2.0",
            "id": self.request_id,
        }

        try:
            response = requests.post(
                self.server_url,
                json=payload,
                headers=self.headers
            )
            response.raise_for_status()

            data = response.json()
            
            if 'error' in data:
                raise ValueError(f"RPC Error: {data['error']}")
            
            return data.get('result')

        except requests.exceptions.RequestException as e:
            raise ConnectionError(f"Network error calling {self.server_url}: {e}") from e
        except json.JSONDecodeError as e:
            raise ValueError(f"Failed to decode JSON response: {e}") from e

# --- Example of how to use the class ---
if __name__ == "__main__":
    #"http://localhost:8080"
    unity_client = JsonRpcClient("127.0.0.1", 8080)

    try:
        print("Calling 'SayHello'...")
        result = unity_client.call("SayHello", {"name": "Python Client"})
        result = unity_client.call("MoveObject", {"x": 30, "y": 50})
        print(f"Response: '{result}'")

    except (ConnectionError, ValueError) as e:
        print(f"\nAn error occurred: {e}")
        print("Please make sure the Unity server is running correctly.")