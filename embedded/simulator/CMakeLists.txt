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
