import os
import sys
import random 

os.chdir("\\".join(sys.argv[0].split("/")[:-1]))
# 189.275 47.31875
bpm = 34.9
secondsPerBeat = 60 / bpm
t1 = []
t2 = []

with open("convertOut.txt", "r") as dataIn:
    lines = dataIn.readlines()


    random.shuffle(lines)

    print(len(lines))
    middle = len(lines) // 2
    count = 0

    for line in lines:

        if count < middle:
            t1.append((float(float(line) / secondsPerBeat)))
        else:
            t2.append((float(float(line) / secondsPerBeat)))

        count += 1

print(len(t1))
print(len(t2))

t1.sort()
t2.sort()

input("the lengths are above, paste the asset data from your scriptableObject")

with open("Bass.asset", "r") as dataIn:
    lines = dataIn.readlines()

    t1c = 0
    t2c = 0
    for lineNum in range(len(lines)):
        if lines[lineNum].strip("\n") == "    - note: 0":
            if t1c <= len(t1) - 1:
                lines[lineNum] = "    - note: " + str(t1[t1c]) + "\n"
                t1c += 1
            else:
                lines[lineNum] = "    - note: " + str(t2[t2c]) + "\n"
                t2c += 1

    with open("Bass.asset.out", "w") as write:
        write.writelines(lines)
        write.close()
    dataIn.close()
