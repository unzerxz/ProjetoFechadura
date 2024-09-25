#include <WiFi.h>
#include <HTTPClient.h>
#include <Keypad.h>
#include <SPI.h>
#include <MFRC522.h>  // Biblioteca para RFID

// Definições para o RFID
#define SS_PIN    5   // Pino SDA do módulo RFID
#define RST_PIN   4   // Pino RST do módulo RFID
MFRC522 rfid(SS_PIN, RST_PIN); // Instância do RFID

// Definições para o Teclado
#define ROW_NUM     4 // Quatro Linhas
#define COLUMN_NUM  4 // Quatro Colunas

char keys[ROW_NUM][COLUMN_NUM] = {
  {'1', '2', '3', 'A'},
  {'4', '5', '6', 'B'},
  {'7', '8', '9', 'C'},
  {'*', '0', '#', 'D'}
};

byte pin_rows[ROW_NUM]      = {13, 12, 14, 27}; // Pinos das linhas do teclado
byte pin_column[COLUMN_NUM] = {26, 25, 33, 32}; // Pinos das colunas do teclado

Keypad keypad = Keypad(makeKeymap(keys), pin_rows, pin_column, ROW_NUM, COLUMN_NUM);

// Conexão WiFi
const char* ssid = "Wokwi-GUEST";  // Coloque seu SSID aqui
const char* password = "";         // Senha do WiFi

// Endpoints da API
String apiUrlValidarEntradaSaida = "http://192.168.1.100:5222/api/validarEntradaSaida";  // Substitua pelo seu IP
String apiUrlAutorizarEntrada = "http://192.168.1.100:5222/api/autorizar-entrada";       // Substitua pelo seu IP

// Variáveis de controle
String inputPassword = "";
long lastInteractionTime = 0;
const int timeoutPeriod = 10000; // 10 segundos de timeout

void setup() {
  Serial.begin(115200);
  
  // Conectar ao WiFi
  connectWiFi();

  // Inicializar RFID
  SPI.begin();
  rfid.PCD_Init();
  
  // Reset da senha
  inputPassword = "";
}

void loop() {
  char key = keypad.getKey();
  if (key) {
    handleKeyInput(key);
    lastInteractionTime = millis();  // Resetar o tempo de interação
  }
  
  if (millis() - lastInteractionTime > timeoutPeriod) {
    resetInputs();  // Reseta a senha se passar 10 segundos sem interação
  }

  // Leitura do RFID
  if (rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial()) {
    String rfidData = getRFIDCredential();
    processRFIDCredential(rfidData);
    lastInteractionTime = millis();
  }
}

// Função para conectar ao WiFi
void connectWiFi() {
  WiFi.begin(ssid, password);
  Serial.print("Conectando ao WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println();
  Serial.println("Conectado ao WiFi!");
}

// Função para tratar a entrada do teclado
void handleKeyInput(char key) {
  if (key >= '0' && key <= '9') {
    inputPassword += key;
    Serial.println(inputPassword);
    
    if (inputPassword.length() == 6) {
      // Senha completa, chamar a API
      validateEntry(inputPassword.toInt());
      resetInputs();  // Resetar senha após envio
    }
  } else if (key == 'A') {
    resetInputs();  // Limpar a entrada ao pressionar 'A'
  }
}

// Função para obter o dado do RFID
String getRFIDCredential() {
  String rfidCredential = "";
  for (byte i = 0; i < rfid.uid.size; i++) {
    rfidCredential += String(rfid.uid.uidByte[i], HEX);
  }
  rfid.PICC_HaltA();  // Parar a leitura do cartão
  return rfidCredential;
}

// Função para processar a credencial RFID
void processRFIDCredential(String rfidData) {
  Serial.print("RFID Lido: ");
  Serial.println(rfidData);
  
  if (rfidData.indexOf("SN792") != -1) {
    // Credencial de usuário
    validateEntryWithCard(rfidData);
  } else if (rfidData.indexOf("792SN") != -1) {
    // Credencial de sala
    authorizeRoomEntry(rfidData);
  }
}

// Função para validar a entrada com a senha (teclado)
void validateEntry(int password) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String apiEndpoint = apiUrlValidarEntradaSaida + "?idSala=1&credencialTeclado=" + password;
    
    http.begin(apiEndpoint);
    int httpCode = http.GET();
    
    if (httpCode > 0) {
      String response = http.getString();
      Serial.println("Resposta da API:");
      Serial.println(response);
    } else {
      Serial.println("Erro na solicitação HTTP.");
    }
    http.end();
  }
}

// Função para validar a entrada com credencial de cartão
void validateEntryWithCard(String cardCredential) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String apiEndpoint = apiUrlValidarEntradaSaida + "?idSala=1&credencialCartao=" + cardCredential;
    
    http.begin(apiEndpoint);
    int httpCode = http.GET();
    
    if (httpCode > 0) {
      String response = http.getString();
      Serial.println("Resposta da API:");
      Serial.println(response);
    } else {
      Serial.println("Erro na solicitação HTTP.");
    }
    http.end();
  }
}

// Função para autorizar entrada na sala com cartão de sala
void authorizeRoomEntry(String roomCredential) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String apiEndpoint = apiUrlAutorizarEntrada + "?idSala=1&credencial=" + roomCredential;
    
    http.begin(apiEndpoint);
    int httpCode = http.GET();
    
    if (httpCode > 0) {
      String response = http.getString();
      Serial.println("Resposta da API:");
      Serial.println(response);
    } else {
      Serial.println("Erro na solicitação HTTP.");
    }
    http.end();
  }
}

// Função para resetar as entradas
void resetInputs() {
  Serial.println("Resetando entradas...");
  inputPassword = "";
}
