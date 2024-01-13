// Usage
string otherClientIP = "50.46.242.76"; // Replace with actual IP
int otherClientPort = 12345; // Replace with actual port
NatPunchthroughClient client = new NatPunchthroughClient(otherClientIP, otherClientPort);
client.StartPunchthroughProcess();