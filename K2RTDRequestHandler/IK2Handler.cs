using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2RTDRequestHandler
{
    /// <summary>
    /// Defines the states a publisher or other object can have
    /// </summary>
    public enum Status
    {
        loaded, opening, open, closed, closing, error, undefined
    }

    /// <summary>
    /// Defines an object that can publish data about some specific topic
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Subscribe the observer specified to the subject
        /// </summary>
        /// <param name="mySubscriber">some susbcriber interface</param>
        void Subscribe(ISubscriber mySubscriber);

        /// <summary>
        /// Unsubscribe the subscriber passed
        /// </summary>
        /// <param name="mySubscriber">some subscriber</param>
        void UnSubscribe(ISubscriber mySubscriber);

        /// <summary>
        /// Return a key based on the state data passed in
        /// </summary>
        /// <param name="myData"></param>
        /// <returns></returns>
        string TopicID(string myData);

        /// <summary>
        /// Return a key for the this subject that can be used to
        /// look up the subject in some map of subjects
        /// </summary>
        /// <returns></returns>
        string TopicID();

        /// <summary>
        /// Open the subject passing data used by this type
        /// of subject.
        /// </summary>
        /// <param name="myData">Data (XML) used by the subject</param>
        /// <returns>A key that can be used to lookup the subject</returns>
        string Open(string myData);

        /// <summary>
        /// Close this subject
        /// </summary>
        void Close();

        /// <summary>
        /// Get/Set the fields list for the publisher - setting this will replace all
        /// existing fields in the publisher and issue an image
        /// </summary>
        System.Collections.Generic.List<Field> FieldList
        {
            get;
            set;
        }
        /// <summary>
        /// Called by a client/data feed with a complete set of fields - this sets up an image in the subject
        /// </summary>
        /// <param name="itemList">list of items</param>
        void OnImage(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called by a client/data feed when one or more fields value changes
        /// </summary>
        /// <param name="itemList">list of changed items</param>
        void OnUpdate(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// update some arbitary field in the publisher - note not all publishers
        /// may support this
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        void OnUpdate(string myID, string myValue);

        /// <summary>
        /// Update the publisher with some price update - not all publishers
        /// will action this.
        /// </summary>
        /// <param name="pxUpdate"></param>
        void OnUpdate(string mnemonic, PXUpdate pxUpdate);

        /// <summary>
        /// Called when the client/datafeed status changes
        /// </summary>
        /// <param name="itemList"></param>
        void OnStatusChange(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// get/set the publisher base status - will event all subscribers
        /// </summary>
        Status Status
        { get; set; }

        /// <summary>
        /// User defined string tag
        /// </summary>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the publisher type - this is user defined
        /// </summary>
        string PublisherType
        { get; set; }
    }

    public enum PublisherType { General, Price };

    /// <summary>
    /// Defines the interface of an object that can manage a set of publishers
    /// </summary>
    public interface IPublisherManager
    {
        /// <summary>
        /// Add/replace a publisher to the manager
        /// </summary>
        /// <param name="myPub"></param>
        void Add(IPublisher myPub);

        /// <summary>
        /// remove a publisher
        /// </summary>
        /// <param name="myPub"></param>
        void Remove(IPublisher myPub);

        /// <summary>
        /// Get the publisher for the TopicID, create one if needs be
        /// </summary>
        /// <param name="myType">Defines the type of subject to Get</param>
        /// <param name="myTopicID">Provides the topic id for the publisher</param>
        /// <param name="register">if true then the publisher is registered with the manager, if not it is simply created</param>
        /// <returns></returns>
        IPublisher GetPublisher(string myType, string myTopicID, bool register);

        /// <summary>
        /// Get a publisher for the topic specified - return null if not found
        /// </summary>
        /// <param name="myTopicID"></param>
        /// <returns>null => not found</returns>
        IPublisher GetPublisher(string myTopicID);

        /// <summary>
        /// Add an instance factory to the publisher manager - this lets 3rd parties
        /// add new publishers
        /// </summary>
        /// <param name="publisherType">price or general</param>
        /// <param name="publisherFactory">factory that can create publishers</param>
        void AddInstanceFactory(PublisherType publisherType, K2InstanceFactory publisherFactory);

        /// <summary>
        /// Get a list of all the available publisher types
        /// </summary>
        /// <returns></returns>
        List<string> GetPublisherTypes();

        /// <summary>
        /// Get a list of all the available publisher topics
        /// </summary>
        /// <returns>list of topics</returns>
        List<string> GetPublisherSubjects();
    }

    public interface ISubscriber
    {
        /// <summary>
        /// Called by a publisher with a complete set of fields - always the
        /// first message after a a subscriber  subcribes to some topic
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of fields and values</param>
        void OnImage(IPublisher mySender, System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called by a publisher when one or more fields value changes
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of changed feilds</param>
        void OnUpdate(IPublisher mySender, System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called when the subject status changes
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of status fields</param>
        void OnStatusChange(IPublisher mySender, System.Collections.Generic.List<Field> itemList);
    }

    /// <summary>
    /// Defines some data field
    /// </summary>
    public interface Field
    {
        /// <summary>
        /// Current value of the field
        /// </summary>
        string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Field name/ID
        /// </summary>
        string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Has the field been changed
        /// </summary>
        bool Changed
        {
            get;
            set;
        }

        /// <summary>
        /// Is the field in a valid state
        /// </summary>
        bool IsValid
        {
            get;
        }
    }

    /// <summary>
    /// Provides a general purpose method to create instances of named objects
    /// </summary>
    public interface K2InstanceFactory
    {
        /// <summary>
        /// Create an instance of the names object - this should a name on the list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object CreateInstance(string name);

        /// <summary>
        /// Get List of all named objects supported - these are the names that will be passed back
        /// to the factory when CreateInstance is called
        /// </summary>
        List<string> Names
        { get; }
    }

    /// <summary>
    /// used to show the atate of the source of pxupdates
    /// </summary>
    public enum PXSourceState { stopped, running, error, endOfData, none };

    /// <summary>
    /// Determines the type of depth operation on an update
    /// </summary>
    public enum PXDepthOperation { insert, delete, update, none };

    /// <summary>
    /// Type of update
    /// </summary>
    public enum PXUpdateType { bidask, bid, ask, trade, image, none };

    public delegate void PXUpdated(PXUpdate pxUpdate);
    public delegate void PXSourceStateChanged(PXSourceState state);

    public class PXFields
    {
        public const int SEQNUMBER = 0;
        public const int TIMETICKS = 1;
        public const int OFFERPRICE = 2;
        public const int OFFERSIZE = 3;
        public const int BIDSIZE = 4;
        public const int BIDPRICE = 5;
        public const int TRADEPRICE = 6;
        public const int TRADEVOLUME = 7;
        public const int OFFERPRICEDELTA = 8;
        public const int OFFERSIZEDELTA = 9;
        public const int BIDSIZEDELTA = 10;
        public const int BIDPRICEDELTA = 11;
        public const int TRADEPRICEDELTA = 12;
        public const int TRADEVOLUMEDELTA = 13;
        public const int DAYHIGH = 14;
        public const int DAYLOW = 15;

        public const int MAXPXFIELDS = 17;
    }

    /// <summary>
    /// This interface represents a price update - note that this is often applied to the L1PX interface
    /// and that its possible to get updates containing the same values for some of the fields
    /// where some price source soes not explicitly produce just the changed price fields
    /// </summary>
    public interface PXUpdate
    {
        /// <summary>
        /// Provider of the update - normally the driver ID
        /// </summary>
        string Originator
        {
            get;
            set;
        }

        /// <summary>
        /// Sequence number of the update - this is unique within a particular susbcription
        /// </summary>
        long Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Time in ticks - local time update received
        /// </summary>
        long Ticks
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the type of price update
        /// </summary>
        PXUpdateType UpdateType
        { get; set; }

        /// <summary>
        /// Time in ticks of the update from the server - this is not always available
        /// </summary>
        long? ServerTicks
        {
            get;
            set;
        }

        /// <summary>
        /// Product mnemonic
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// Market (MarketMaker) offering a depth entry
        /// </summary>
        string DepthMarket
        {
            get;
            set;
        }
        /// <summary>
        /// position of the depth  0 ... +N, not that the use of bid or offer price
        /// implies the side
        /// </summary>
        int DepthPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the depth operation - i.e insert, replace delete
        /// </summary>
        PXDepthOperation DepthOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Tag/data that can be added by a driver
        /// </summary>
        string DriverTag
        {
            get;
            set;
        }

        decimal? OfferPrice
        {
            get;
            set;
        }

        decimal? OfferSize
        {
            get;
            set;
        }

        decimal? BidSize
        {
            get;
            set;
        }

        decimal? BidPrice
        {
            get;
            set;
        }

        decimal? TradePrice
        {
            get;
            set;
        }

        decimal? TradeVolume
        {
            get;
            set;
        }

        decimal? DayHigh
        {
            get;
            set;
        }

        decimal? DayLow
        {
            get;
            set;
        }

        /// <summary>
        /// Current delta
        /// </summary>
        decimal? OfferPriceDelta
        {
            get;
            set;
        }

        /// <summary>
        /// Curreent delta
        /// </summary>
        decimal? OfferSizeDelta
        {
            get;
            set;
        }

        decimal? BidSizeDelta
        {
            get;
            set;
        }

        decimal? BidPriceDelta
        {
            get;
            set;
        }
        decimal? TradePriceDelta
        {
            get;
            set;
        }
        decimal? TradeVolumeDelta
        {
            get;
            set;
        }

        void CalculateDeltas(PXUpdate prevUpdate);

        /// <summary>
        /// Sets the current update using an existing update
        /// </summary>
        /// <param name="update"></param>
        void From(PXUpdate update);

        

        //void To(L1PX myL1PX);

        void From(string myData, char myDelimiter);

        string To(char myDelimiter);
    }
}
