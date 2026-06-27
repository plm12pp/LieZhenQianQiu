#!/usr/bin/env python3
import http.server
import ssl
import os

os.chdir('/workspace')
server_address = ('0.0.0.0', 8443)
httpd = http.server.HTTPServer(server_address, http.server.SimpleHTTPRequestHandler)
ctx = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
ctx.load_cert_chain('server.crt', 'server.key')
httpd.socket = ctx.wrap_socket(httpd.socket, server_side=True)
print(f'HTTPS server running on port 8443')
httpd.serve_forever()
