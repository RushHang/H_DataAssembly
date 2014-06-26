using System.IO;

/// <summary>
/// Gifʹ�õĿɱ䳤�ȵ�LZWѹ���㷨������
/// </summary>
public class LZWDecoder
{
    #region ����
    /// <summary>
    /// GIF�涨�������Ϊ12bit�����ֵ��Ϊ4096
    /// </summary>
    protected static readonly int MaxStackSize = 4096;
    protected Stream stream;
    #endregion

    #region ���캯��
    /// <summary>
    /// ���캯��
    /// </summary>
    public LZWDecoder(Stream stream)
    {
        this.stream = stream;
    }
    #endregion

    #region ˽�з���
    /// <summary>
    /// ��ȡ���ݶ�
    /// </summary>
    byte[] ReadData()
    {
        int blockSize = Read();
        return ReadByte(blockSize);
    }

    /// <summary>
    /// ��ȡָ�����ȵ��ֽ��ֽ�
    /// </summary>
    byte[] ReadByte(int len)
    {
        byte[] buffer = new byte[len];
        stream.Read(buffer, 0, len);
        return buffer;
    }

    /// <summary>
    /// ��ȡһ���ֽ�
    /// </summary>
    int Read()
    {
        return stream.ReadByte();
    }
    #endregion

    #region ���÷���
    /// <summary>
    /// LZWѹ���㷨������
    /// </summary>
    /// <param name="width">����</param>
    /// <param name="height">�߶�</param>
    /// <param name="stream">������������������</param>
    /// <returns>ԭʼ������</returns>
    public byte[] DecodeImageData(int width, int height, Stream stream)
    {
        int NullCode = -1;
        int pixelCount = width * height;//��ȡԭͼ�������������ʽΪ ������ = ͼ�񳤶�*ͼ��߶�
        byte[] pixels = new byte[pixelCount];
        int dataSize = Read();          //ͼ��������ĵ�һ���ֽ�(byte)��ŵ�������λ��С����gifͨ��Ϊ1,4,8
        int codeSize = dataSize + 1;    //����λ��С������lzw�㷨��Ҫ�󣬱���λ�Ĵ�С = ����λ��С+1 �����gif�������¶�Ӧ��ϵ 1->3 4->5 ->9,������codeSizeΪ12
        int clearFlag = 1 << dataSize;  //��lzw�㷨�����������ǣ�clearFlagΪ���е������ǣ��˺�ı��뽫��ͷ���������������Է�ֹ����λ��������
        int endFlag = clearFlag + 1;    //lzw�㷨����������֮һ��endFlagΪ������ǣ���ʾһ�α���Ľ���  endFlag=clearFlag+1
        int available = endFlag + 1;    //��ʼ�Ŀ��ñ����С����Ϊ0-(clear-1)ΪԪ���ݣ����Ծ����ã������о����˴������γ�ѹ���ı��뿪ʼ����

        int code = NullCode;     //���ڴ洢��ǰ�ı���ֵ
        int old_code = NullCode; //���ڴ洢��һ�εı���ֵ
        int code_mask = (1 << codeSize) - 1;//��ʾ��������ֵ�����codeSize=5,��code_mask=31
        int bits = 0;//�ڱ����������ݵı�����ʽΪbyte����ʵ�ʱ������������ʵ�ʱ���λ���洢�ģ����統codeSize=5��ʱ����ôʵ����5bit�����ݾ�Ӧ�ÿ��Ա�ʾһ�����룬����ȡ������1���ֽھ͸�����3��bit����3��bit���ں͵ڶ����ֽڵĺ�����bit������ϣ��ٴ��γɱ���ֵ���������

        int[] prefix = new int[MaxStackSize];          //���ڱ���ǰ׺�ļ���
        int[] suffix = new int[MaxStackSize];          //���ڱ����׺
        int[] pixelStatck = new int[MaxStackSize + 1]; //������ʱ����������

        int top = 0;
        int count = 0; //�������ѭ���У�ÿ�λ��ȡһ�����ı�����ֽ����飬��������Щ�����ʱ����Ҫ1�����ֽ�������count���Ǳ�ʾ��Ҫ������ֽ���Ŀ
        int bi = 0;    //count��ʾ��ʣ�����ֽ���Ҫ������bi���ʾ�����Ѿ�����ĸ���
        int i = 0;     //i����ǰ����õ�������

        int data = 0;          //��ʾ��ǰ��������ݵ�ֵ
        int first = 0;         //һ���ַ����صĵ�һ���ֽ�
        int inCode = NullCode; //��lzw�У������ʶ��һ�����������������entry���򽫱�����Ϊ��һ�ε�prefix���˴�inCode�����ݸ���һ����Ϊǰ׺�ı���ֵ

        //������Ԫ���ݵ�ǰ׺���Ϻͺ�׺���ϣ�Ԫ���ݵ�ǰ׺��Ϊ0������׺��Ԫ������ȣ�ͬʱ����Ҳ��Ԫ�������
        for (code = 0; code < clearFlag; code++)
        {
            //ǰ׺��ʼΪ0
            prefix[code] = 0;
            //��׺=Ԫ����=����
            suffix[code] = (byte)code;
        }

        byte[] buffer = null;
        while (i < pixelCount)
        {
            //����������Ѿ�ȷ��ΪpixelCount = width * width
            if (top == 0)
            {
                if (bits < codeSize)
                {
                    //�����ǰ��Ҫ�����bit��С�ڱ���λ��С������Ҫ��������
                    if (count == 0)
                    {
                        //���countΪ0����ʾҪ�ӱ������ж�һ�����ݶ������з���
                        buffer = ReadData();
                        count = buffer.Length;
                        if (count == 0)
                        {
                            //�ٴ����ȡ���ݶΣ�ȴû�ж������ݣ���ʱ�ͱ����Ѿ���������
                            break;
                        }
                        //���¶�ȡһ�����ݶκ�Ӧ�ý��Ѿ�����ĸ�����0
                        bi = 0;
                    }
                    //��ȡ����Ҫ��������ݵ�ֵ
                    data += buffer[bi] << bits;//�˴�Ϊ��Ҫ��λ�أ������һ�δ�����1���ֽ�Ϊ176����һ��ֻҪ����5bit�͹��ˣ�ʣ��3bit�����¸��ֽڽ�����ϡ�Ҳ���ǵڶ����ֽڵĺ���λ+��һ���ֽڵ�ǰ��λ��ɵڶ������ֵ
                    bits += 8; //�����ִ�����һ���ֽڣ�������Ҫ+8
                    bi++;      //��������һ���ֽ�
                    count--;   //�Ѿ���������ֽ���+1
                    continue;
                }

                //����Ѿ����㹻��bit���ɹ�����������Ǵ������
                code = data & code_mask; //��ȡdata���ݵı���λ��Сbit������
                data >>= codeSize;       //���������ݽ�ȡ��ԭ�������ݾ�ʣ�¼���bit�ˣ���ʱ����Щbit���ƣ�Ϊ�´���׼��
                bits -= codeSize;        //ͬʱ��Ҫ����ǰ���ݵ�bit����ȥ����λ������Ϊ�Ѿ��õ��˴���

                //������ݻ�ȡ��codeֵ�����д���
                if (code > available || code == endFlag)
                {
                    //������ֵ����������ֵ����Ϊ������ǵ�ʱ��ֹͣ����
                    break;
                }
                if (code == clearFlag)
                {
                    //�����ǰ�������ǣ������³�ʼ������������������
                    codeSize = dataSize + 1;
                    //���³�ʼ��������ֵ
                    code_mask = (1 << codeSize) - 1;
                    //��ʼ����һ��Ӧ�ô���ñ���ֵ
                    available = clearFlag + 2;
                    //�����浽old_code�е�ֵ������Ա���ͷ����
                    old_code = NullCode;
                    continue;
                }

                //������code������ѹ���ı��뷶Χ�ڵĵĴ������
                if (old_code == NullCode)
                {
                    //�����ǰ����ֵΪ��,��ʾ�ǵ�һ�λ�ȡ����
                    pixelStatck[top++] = suffix[code];//��ȡ��1��������������
                    //���α��봦����ɣ�������ֵ���浽old_code��
                    old_code = code;
                    //��һ���ַ�Ϊ��ǰ����
                    first = code;
                    continue;
                }
                inCode = code;
                if (code == available)
                {
                    //�����ǰ����ͱ���Ӧ�����ɵı�����ͬ
                    pixelStatck[top++] = (byte)first;//��ô��һ�������ֽھ͵��ڵ�ǰ�����ַ����ĵ�һ���ֽ�
                    code = old_code; //���ݵ���һ������
                }
                while (code > clearFlag)
                {
                    //�����ǰ������������ǣ���ʾ����ֵ����ѹ�����ݵ�
                    pixelStatck[top++] = suffix[code];
                    code = prefix[code];//���ݵ���һ������
                }
                first = suffix[code];
                if (available > MaxStackSize)
                {
                    //���������ֵ����gif������ı��루4096�����ֵ��ʱ��ֹͣ����
                    break;
                }

                //��ȡ��һ������
                pixelStatck[top++] = suffix[code];
                //���õ�ǰӦ�ñ���λ�õ�ǰ׺
                prefix[available] = old_code;
                //���õ�ǰӦ�ñ���λ�õĺ�׺
                suffix[available] = first;
                //�´�Ӧ�õõ��ı���ֵ
                available++;
                if (available == code_mask + 1 && available < MaxStackSize)
                {
                    //���ӱ���λ��
                    codeSize++;
                    //����������ֵ
                    code_mask = (1 << codeSize) - 1;
                }
                //��ԭold_code
                old_code = inCode;
            }
            //���ݵ���һ������λ��
            top--;
            //��ȡԪ����
            pixels[i++] = (byte)pixelStatck[top];
        }
        return pixels;
    }
    #endregion
}

//LZW����ѹ���㷨��ԭ�������http://www.cnblogs.com/jillzhang/archive/2006/11/06/551298.html