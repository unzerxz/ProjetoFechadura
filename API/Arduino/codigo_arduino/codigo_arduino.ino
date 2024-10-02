#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <Keypad.h>
#include <SPI.h>
#include <MFRC522.h>  // Biblioteca para RFID

// Definições para o RFID
#define SS_PIN    5   // Pino SDA do módulo RFID
#define RST_PIN   22  // Pino RST do módulo RFID
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
const char* ssid = "WIFI-IOT";  // Coloque seu SSID aqui
const char* password = "ac7ce9ss2@iot";         // Senha do WiFi

// Endpoints da API
String apiUrlValidarEntradaSaida = "http://200.210.232.98:5222/api/validarEntradaSaida";  // Substitua pelo seu IP
String apiUrlAutorizarEntrada = "http://200.210.232.98:5222/api/autorizar-entrada";       // Substitua pelo seu IP

// Variáveis de controle
String inputPassword = "";
long lastInteractionTime = 0;
const int timeoutPeriod = 10000; // 10 segundos de timeout

// Protótipos das funções
void connectWiFi();
void handleKeyInput(char key);
String getRFIDCredential();
void processRFIDCredential(String rfidData);
void validateEntry(int password);
void validateEntryWithCard(String cardCredential);
void authorizeRoomEntry(String roomCredential);
void resetInputs();

void setup() {
  Serial.begin(115200);
  
  // Conectar ao WiFi
  connectWiFi();

  // Inicializar RFID
  SPI.begin();
  rfid.PCD_Init();
  
  // RFID module version check
  byte version = rfid.PCD_ReadRegister(MFRC522::VersionReg);
  if (version == 0x00 || version == 0xFF) {
    Serial.println("WARNING: RFID module not detected!");
  } else {
    Serial.print("RFID module detected. Version: 0x");
    Serial.println(version, HEX);
  }

  // Dump RFID module registers for debugging
  rfid.PCD_DumpVersionToSerial();
  
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
  if (rfid.PICC_IsNewCardPresent()) {
    Serial.println("Novo cartão detectado");
    if (rfid.PICC_ReadCardSerial()) {
      Serial.println("Card serial read successfully");
      String rfidData = getRFIDCredential();
      processRFIDCredential(rfidData);
      lastInteractionTime = millis();
    } else {
      Serial.println("Failed to read card serial");
    }
  }

  // Add this to check RFID module status periodically
  static unsigned long lastRFIDCheck = 0;
  if (millis() - lastRFIDCheck > 5000) {  // Check every 5 seconds
    lastRFIDCheck = millis();
    byte v = rfid.PCD_ReadRegister(MFRC522::VersionReg);
    if (v == 0x00 || v == 0xFF) {
      Serial.println("RFID module not detected during periodic check!");
    } else {
      Serial.print("RFID module version (periodic check): 0x");
      Serial.println(v, HEX);
    }
  }
}

// Função para conectar ao WiFi
void connectWiFi() {
  WiFi.begin(ssid, password);
  Serial.print("Conectando ao WiFi");
  int attempts = 0;
  while (WiFi.status() != WL_CONNECTED && attempts < 20) {
    delay(1000);
    Serial.print(".");
    attempts++;
  }
  Serial.println();
  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("Conectado ao WiFi!");
    Serial.print("Endereço IP: ");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("Falha na conexão WiFi. Status: " + String(WiFi.status()));
  }
}

// Função para obter o dado do RFID
String getRFIDCredential() {
  String rfidCredential = "";
  for (byte i = 0; i < rfid.uid.size; i++) {
    rfidCredential += (rfid.uid.uidByte[i] < 0x10 ? "0" : "");
    rfidCredential += String(rfid.uid.uidByte[i], HEX);
  }
  rfid.PICC_HaltA();  // Parar a leitura do cartão
  Serial.print("RFID lido: ");
  Serial.println(rfidCredential);
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
      Serial.print("Erro na solicitação HTTP. Código: ");
      Serial.println(httpCode);
      Serial.println("Erro: " + http.errorToString(httpCode));
    }
    http.end();
  } else {
    Serial.println("Não conectado ao WiFi. Status: " + String(WiFi.status()));
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
      Serial.print("Erro na solicitação HTTP. Código: ");
      Serial.println(httpCode);
      Serial.println("Erro: " + http.errorToString(httpCode));
    }
    http.end();
  } else {
    Serial.println("Não conectado ao WiFi. Status: " + String(WiFi.status()));
  }
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
      Serial.print("Erro na solicitação HTTP. Código: ");
      Serial.println(httpCode);
      Serial.println("Erro: " + http.errorToString(httpCode));
    }
    http.end();
  } else {
    Serial.println("Não conectado ao WiFi. Status: " + String(WiFi.status()));
  }
}

// Função para resetar as entradas
void resetInputs() {
  Serial.println("Resetando entradas...");
  inputPassword = "";
}