namespace CManipulator
{
    using System;

    public class BitManipulator
    {
        public static void WriteBits(byte[] array, int indexBit, int size, int value)
        {
            int indexArray = indexBit / 8;
            int bitStart   = indexBit % 8;
            int countWritten = 0;

            while (countWritten < size)
            {
                int byteCurrent    = array[indexArray];
                int bitsLeftInByte = 8 - bitStart;
                int bitsToWrite = Math.Min(bitsLeftInByte, size - countWritten);

                //해당 비트 구간을 모두 1로 채워서 & ~mask로 비트를 초기화한다.
                int mask = (1 << bitsToWrite) - 1;

                //남은 비트만큼 밀어넣고
                array[indexArray] = (byte)((byteCurrent & ~(mask << bitStart)) | ((value >> countWritten) & mask) << bitStart);

                //인덱스 등을 갱신한 후 다음 byte[] 으로 넘어간다.
                countWritten += bitsToWrite;
                bitStart = 0;
                indexArray++;
            }
        }
        public static int ReadBits(byte[] data, int bitIndex, int bitCount)
        {
            int byteIndex = bitIndex / 8;
            int startBit = bitIndex % 8;
            int result = 0;
            int bitsRead = 0;

            while (bitsRead < bitCount)
            {
                int currentByte = data[byteIndex];
                int bitsLeftInByte = 8 - startBit;
                int bitsToRead = Math.Min(bitsLeftInByte, bitCount - bitsRead);
                int mask = (1 << bitsToRead) - 1;

                result |= ((currentByte >> startBit) & mask) << bitsRead;

                bitsRead += bitsToRead;
                startBit = 0; // For subsequent bytes, start at the beginning
                byteIndex++;
            }

            return result;
        }
    }
}