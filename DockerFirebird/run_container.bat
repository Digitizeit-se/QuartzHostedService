@echo off

REM Only create the network if it does not already exist
set network_name=quartznet
set existing_network=
for /f %%i in ('docker network ls --filter name^=%network_name% --format^="{{ .Name }}"') do set existing_network=%%i
if not "%existing_network%" == "%network_name%" (
  echo Creating network
  docker network create --subnet=172.16.240.0/24 --gateway=172.16.240.254 %network_name%
)
docker-compose -f docker-compose/docker-compose.yml -p firebird up --force-recreate