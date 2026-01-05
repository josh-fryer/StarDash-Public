//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Purchasing;

//public class StoreManager : MonoBehaviour, IStoreListener
//{
//    IStoreController m_StoreController; // The Unity Purchasing system.
//    public bool useFakeStore = true;
//    private string removeAdsID = "removeads";

//    private UIManager uiManager;

//    private void Start()
//    {
//        //InitializePurchasing();
//        uiManager = FindObjectOfType<UIManager>();
//    }

//    //void InitializePurchasing()
//    //{
//    //    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//    //    //Add products that will be purchasable and indicate its type.
//    //    builder.AddProduct(removeAdsID, ProductType.Consumable);

//    //    UnityPurchasing.Initialize(this, builder);

//    //    //StandardPurchasingModule.Instance().useFakeStoreAlways = useFakeStore;
//    //    //StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
//    //}

//    public void OnRemoveAdsPurchaseComplete()
//    {
//        // purchase removaads
//        // 1 = true; 0 = false;
//        PlayerPrefs.SetInt("AdsRemoved", 1);
//        Debug.Log("Removed Ads");

//        uiManager.HideRemoveAdsBtn();
//    }

//    public void PurchaseRemoveAds()
//    {
//        m_StoreController.InitiatePurchase(removeAdsID);
//    }

//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        throw new System.NotImplementedException();
//    }

//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        //Retrieve the purchased product
//        var product = args.purchasedProduct;

//        //Add the purchased product to the players inventory
//        if (product.definition.id == removeAdsID)
//        {
//            OnRemoveAdsPurchaseComplete();
//        }

//        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

//        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
//        return PurchaseProcessingResult.Complete;
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
//    }

//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        Debug.Log("In-App Purchasing successfully initialized");
//        m_StoreController = controller;
//    }
//}
