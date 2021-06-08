using CI.QuickSave;
public class SaveItemWriter{
    private int index;
    private QuickSaveWriter writer;
    public SaveItemWriter(int index, QuickSaveWriter writer){
        this.index = index;
        this.writer = writer;
    }
    public void Save<T>(string property, T value){
        writer.Write("O" + index + "." + property, value);
    }
}
public class SaveItemReader{
    private int index;
    private QuickSaveReader reader;
    public SaveItemReader(int index, QuickSaveReader reader){
        this.index = index;
        this.reader = reader;
    }
    public T Load<T>(string property){
        return reader.Read<T>("O" + index + "." + property);
    }
}