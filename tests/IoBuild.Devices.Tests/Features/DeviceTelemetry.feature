Feature: Telemetria de Dispositivos IoT
  Como un Property Manager
  Quiero consultar telemetria de dispositivos
  Para monitorear el consumo y el estado en tiempo real

  Scenario: Consultar consumo de energia por hora
    Given el usuario esta autenticado como "PropertyManager"
    And existe un dispositivo con ID 1
    And el dispositivo 1 tiene datos de telemetria en las ultimas 24 horas
    When envia GET a "/api/v1/devices/1/energy"
    Then respuesta 200 OK
    And la respuesta contiene una lista de puntos con timestamp, energyKwh, temperatureC, voltageV

  Scenario: Consultar el ultimo estado conocido
    Given el usuario esta autenticado
    And existe un dispositivo con ID 1
    And el dispositivo 1 tiene telemetria con estado "online"
    When envia GET a "/api/v1/devices/1/status"
    Then respuesta 200 OK
    And contiene el estado "online" y el campo lastSeen

  Scenario: Sin datos de telemetria
    Given el usuario esta autenticado
    And existe un dispositivo con ID 1
    And el dispositivo 1 no tiene datos de telemetria
    When envia GET a "/api/v1/devices/1/energy"
    Then respuesta 200 OK y lista vacia

  Scenario: Usuario no autenticado
    Given el usuario no esta autenticado
    When envia GET a "/api/v1/devices/1/energy"
    Then respuesta 401 Unauthorized
