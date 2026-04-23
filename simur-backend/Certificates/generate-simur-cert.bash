# Gera a chave privada e o certificado autoassinado (válido por 10 anos)
openssl req -x509 -nodes -days 3650 -newkey rsa:2048 \
  -keyout simur.key -out simur.crt -config simur-cert.conf -extensions v3_req

# Converte para o formato PFX que o .NET (Kestrel) exige
# Use a senha '12345678' para manter o padrão do seu compose
openssl pkcs12 -export -out aspnetapp.pfx -inkey simur.key -in simur.crt -password pass:12345678