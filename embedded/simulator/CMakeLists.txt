cmake_minimum_required(VERSION 3.16)

project(VehicleBlackBoxSimulator)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(
    vehicle_black_box_simulator
    src/main.cpp
)

target_include_directories(
    vehicle_black_box_simulator
    PRIVATE
    include
)

# --- MQTT (P2.1) ---
set(MOSQUITTO_ROOT "C:/Program Files/mosquitto")

find_library(MOSQUITTO_LIB
    NAMES mosquitto
    PATHS "${MOSQUITTO_ROOT}/devel"
)
find_path(MOSQUITTO_INCLUDE_DIR
    NAMES mosquitto.h
    PATHS "${MOSQUITTO_ROOT}/devel"
)

if(NOT MOSQUITTO_LIB OR NOT MOSQUITTO_INCLUDE_DIR)
    message(FATAL_ERROR "libmosquitto nao encontrada em ${MOSQUITTO_ROOT}/devel. Confirme o caminho da instalacao.")
endif()

target_include_directories(vehicle_black_box_simulator PRIVATE
    ${MOSQUITTO_INCLUDE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/include
    ${CMAKE_CURRENT_SOURCE_DIR}/include/cjson
)

target_sources(vehicle_black_box_simulator PRIVATE
    src/mqtt_client.cpp
    src/cJSON.c
)

target_link_libraries(vehicle_black_box_simulator PRIVATE
    ${MOSQUITTO_LIB}
)