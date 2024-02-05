#!/bin/bash

# 获取环境变量
SERVER_IP=`hostname -I`
SERVER_PORT=${1}
BUILD_COMMAND=${2}
BUILD_ENV=${3}

${BUILD_COMMAND} --environment=${BUILD_ENV:-DEV} --urls=http://*:${SERVER_PORT:-80} --ip=${SERVER_IP} --port=${SERVER_PORT:-80}