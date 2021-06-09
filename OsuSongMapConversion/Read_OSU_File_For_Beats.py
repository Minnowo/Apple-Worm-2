import os
import sys 
import random 

if(len(sys.argv) > 1):
    if(sys.argv[0].find("/") > -1):
        pass
os.chdir("\\".join(sys.argv[0].split("/")[:-1]))


data = []
beginTakingData = False
with open("convert.osu", "r") as dataIn:
    lines = dataIn.readlines()

    for line in lines:
        if line == "[HitObjects]\n":
            beginTakingData = True

        if not beginTakingData or line == "[HitObjects]\n": 
            continue

        data.append(str(float(line.split(",")[2]) / 1000) + "\n")

    dataIn.close()

with open("convertOut.txt", "w") as writer:
    writer.writelines(data)
