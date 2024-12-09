﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Dock.Model.Core;
using Oneware.NetlistReaderFrontend.Controls;
using Oneware.NetlistReaderFrontend.Services;
using Oneware.NetlistReaderFrontend.Types;
using Oneware.NetlistReaderFrontend.ViewModels;
using ReactiveUI;

namespace Oneware.NetlistReaderFrontend.Views;

// Avalonia expects the View corresponding to a given ViewModel to have the same name, but ending in just "View" instead
// of "ViewModel" [1]
//
// [1]: https://docs.avaloniaui.net/docs/concepts/view-locator
public partial class FrontendView : UserControl
{
    private FrontendViewModel? _vm;
    
    public FrontendView()
    {
        InitializeComponent();

        if (DataContext is FrontendViewModel vm)
        {
            Initialize(vm);
        }

        DataContextChanged += OnDataContextChanged;
        
        ServiceManager.GetCustomLogger().Log($"FrontendView Initialized", true);
        ServiceManager.GetCustomLogger().Log($"DataContext: {DataContext}", true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty)
        {
            ServiceManager.GetCustomLogger().Log(e.Property + " - " + e.OldValue + " - " + e.NewValue, true);


            if (_vm == null)
            {
                _vm = e.NewValue as FrontendViewModel;
            }
        }
        
        base.OnPropertyChanged(e);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (IsInitialized)
        {
            _vm = DataContext as FrontendViewModel;
        }
        else
        {
            Initialized += delegate { OnDataContextChanged(sender, e); };
        }
    }

    // move netlist
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var NetlistControlElem = sender as NetlistControl;
        
        var pointerpoints = e.GetIntermediatePoints(NetlistControlElem);
        double dx, dy;

        if (pointerpoints.Count > 1 && pointerpoints.First().Properties.IsLeftButtonPressed)
        {
            dx = pointerpoints.Last().Position.X -  pointerpoints.First().Position.X;
            dy = pointerpoints.Last().Position.Y -  pointerpoints.First().Position.Y;
        }
        else
        {
            dx = 0;
            dy = 0;
        }

        NetlistControlElem.DeltaX += dx;
        NetlistControlElem.DeltaY += dy;
        NetlistControlElem.DeltaScale = 0;
    }

    // change zoom
    private void InputElement_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var wheel = e.Delta;
        var NetlistControlElem = sender as NetlistControl;

        // only react to vertical scrolling for now
        NetlistControlElem.DeltaScale += wheel.Y;
        
        NetlistControlElem.PointerX = e.GetPosition(NetlistControlElem).X;
        NetlistControlElem.PointerY = e.GetPosition(NetlistControlElem).Y;
    }

    private void NetlistControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Get click event to Control
        // There should be a better way
        
        ((NetlistControl) sender).NetlistControl_PointerPressed(sender, e);
    }

    private void NetlistControl_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ((NetlistControl) sender).NetlistControl_PointerReleased(sender, e);
    }

    private void NetlistControl_OnTapped(object? sender, TappedEventArgs e)
    {
        Console.WriteLine("Tap!");
        
        ((NetlistControl) sender).NetlistControl_OnTapped(sender, e);
    }

    private void NetlistControl_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        ((NetlistControl) sender).Redraw();
    }

    private void Initialize(FrontendViewModel vm)
    {
        ServiceManager.GetCustomLogger().Log($"New Frontend: {_vm == vm}", true);
        ServiceManager.GetCustomLogger().Log($"New NetlistID: {vm.NetlistId}", true);
        ServiceManager.GetCustomLogger().Log($"Old NetlistID: {_vm?.NetlistId}", true);
        
        
        _vm = vm;
    }

    private void NetlistControl_OnElementClicked(object sender, ElementClickedEventArgs e)
    {
        ServiceManager.GetCustomLogger().Log(e.NodePath, true);
    }
}