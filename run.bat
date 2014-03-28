@echo off set master=./Master/bin/Release/*.exe
start %master%

@echo off set server=./Sever/bin/Release/*.exe
start %server% 8081
start %master% 8082
start %master% 8083
start %master% 8085
start %master% 8086

@echo off set client=./Client/bin/Release/*.exe
start %client%