using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2RTDServerKit
{
    /// <summary>
    /// Delegate called to handle comms status updates
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnStatusUpdate(object sender, StatusEventArgs e);

    /// <summary>
    /// Delegate to handle requests sent form Excel using RTD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnRequest(object sender, RequestEventArgs e);

    /// <summary>
    /// Defines the behaviour and properties of some K2 RTD Server, this interface is used
    /// by people writing a server that is called from Excel RTD
    /// </summary>
    public interface IRTDServer
    {
        /// <summary>
        /// convert a string array to a tab delimted string
        /// </summary>
        /// <param name="myStringArray"></param>
        /// <returns></returns>
        string GetAsString(string[] myStringArray);

        /// <summary>
        /// Get a list of the available subjects
        /// </summary>
        /// <returns>List of subject names</returns>
        System.Collections.Generic.List<string> GetSubjects();

        /// <summary>
        /// Get/Set handlers for the requests from Excel
        /// </summary>
        OnRequest OnRequestHandler { get; set; }

        /// <summary>
        /// Get/Set Handlers for status updates
        /// </summary>
        OnStatusUpdate OnStatusUpdateHandler { get; set; }

        /// <summary>
        /// publish an array of double values
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="values">array of double values</param>
        void Publish(string subject, double[] values);

        /// <summary>
        /// publish an array of int values
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="values">array of int values</param>
        void Publish(string subject, int[] values);

        /// <summary>
        /// Publish an update for the subject and header specified
        /// </summary>
        /// <param name="subject">subject that you want to publish data against - e.g. IBM</param>
        /// <param name="header">header that the data is for - e.g. QUANTITY </param>
        /// <param name="value">value you want to publish</param>
        void Publish(string subject, string header, string value);

        /// <summary>
        /// Start the handler running - will allow KaiTrade RTDs to connect to the server
        /// </summary>
        /// <param name="myState">for future use</param>
        void StartHandler(string myState);

        /// <summary>
        /// Stop the hander - all sessions will be closed
        /// </summary>
        void StopHandler();


        /// <summary>
        /// Subscribe based on the topic def and monitor changes for the correlation ID
        /// if the subject is open then the corrID will be added to the interested parties
        /// </summary>
        /// <param name="myTopicDef">This is the identifer for the product or subject required</param>
        /// <param name="myCorrelationID">used to identify the channel used to respond on</param>
        void Subscribe(string subject, string headerName, int excelID, string accessID);

        /// <summary>
        /// Single topic update- this allows you to explicitly set
        /// some Excel topics. Note that in general you should use a publish method
        /// </summary>
        /// <param name="accessID">access id for the particular instance of Excel</param>
        /// <param name="id">excel topic id</param>
        /// <param name="value">value</param>
        void TopicUpdate(string accessID, int id, string value);

        /// <summary>
        /// Update a set of topics in Excel - this allows you to explicitly set
        /// some Excel topics. Note that in general you should use a publish method
        /// </summary>
        /// <param name="accessID">access id for the particular instance of Excel</param>
        /// <param name="id">array of excel topic ids to update</param>
        /// <param name="value">array of values to write into the topics</param>
        void TopicUpdate(string accessID, int[] id, string[] value);
    }
}
