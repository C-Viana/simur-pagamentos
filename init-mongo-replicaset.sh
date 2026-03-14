#!/bin/bash

until mongosh --host mongo1 --eval 'print("waiting")'; do sleep 2; done

mongosh --host mongo1 --eval 'rs.initiate({_id: "rs0", members: [{_id: 0, host: "mongo1:27017"}]})'