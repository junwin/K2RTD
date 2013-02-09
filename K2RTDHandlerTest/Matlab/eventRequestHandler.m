% Save this as an eventRequestHandler.m file
function eventRequestHandler(source,arg)
disp(arg.rtdTopicID)    
topicID = arg.parameters(2);      
reqType = arg.parameters(3);      
subject = arg.parameters(4);     
headerName = arg.parameters(5);  
disp(topicID)
disp(reqType)
disp(subject)
disp(headerName)
reqType =char(reqType);
disp(reqType)
if strcmp(reqType,'PX')
    % This will register this in the Servers publisher list
    source.Subscribe(subject, headerName, arg.rtdTopicID, arg.accessID);
elseif strcmp(reqType,'WPUB')
    % WPUB is used to publish data into Matlab
    % note you can have an arbitary length of data values
    dataValue = arg.parameters(6);
    disp(dataValue)
end
end