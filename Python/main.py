# GAME TIME READER
# WINDSWEPT DEMO ASM 1.0.0.0

# 1. "Windswept.exe" + 00A421F0 -> GET VALUE AS 4 BYTE IN HEX (EX: 10)
# 2. Add Offset to VALUE, read memory location at VALUE
# 3. REPEAT UNTIL OFFSET SIX. Read memory location at value
# 4. Memory location at value at OFFSET_SIX is STAGE TIME

from ReadWriteMemory import ReadWriteMemory
import time
import win32api
import win32process

EXECUTABLE = "Windswept.exe"
POINTER = 0x00A421F0
OFFSETS = [
    0x10, 0x80, 0x48, 0x10, 0x930, 0x300
]

def convertMillis(millis):
    seconds=(millis/1000)%60
    minutes=(millis/(1000*60))%60
    hours=(millis/(1000*60*60))%24
    return seconds, minutes, hours

def get_base_address(pid: int):
    PROCESS_ALL_ACCESS = 0x1F0FFF
    processHandle = win32api.OpenProcess(PROCESS_ALL_ACCESS, False, pid)
    modules = win32process.EnumProcessModules(processHandle)
    processHandle.close()
    base_addr = modules[0] 

    return base_addr


def main():
    rwm = ReadWriteMemory()
    process = rwm.get_process_by_name(EXECUTABLE)
    pid = process.__dict__['pid']

    print("Process PID: ", pid)
    
    base_addr = get_base_address(pid)

    print ("Base Addr:", base_addr)

    try:
        process.open()
    except Exception as ex:
        print("Failed to open process with exception ", ex)
        quit()
    
    print("Successfully opened process!")
    
    time_pointer = process.get_pointer(base_addr+POINTER, offsets=OFFSETS)
    
    while(True):
        stageTime = process.readDouble(time_pointer)
        sec, min, hours = convertMillis(stageTime)
        print("Time: %d:%d:%d" %(hours, min, sec))
        time.sleep(0.5)



if __name__ == '__main__':
    main()
