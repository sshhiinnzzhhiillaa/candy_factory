using _task8.Domain;
using _task8.Domain.Conveyors;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using _task8.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace _task8
{
    public class MainViewModel : ViewModelBase
    {
        public Factory Factory { get; set; } = new Factory();

        public List<Type> ConveyorsOptions { get; } = new List<Type> { typeof(OldConveyor), typeof(NewConveyor) };

        public ObservableCollection<IConveyor> Conveyors { get; set; } = new ObservableCollection<IConveyor> { };

        public RelayCommand AddCommand { get; private set; }

        public RelayCommand<IConveyor> StartCommand { get; private set; }

        public RelayCommand<IConveyor> StopCommand { get; private set; }

        public RelayCommand<IConveyor> DeleteCommand { get; private set; }

        public RelayCommand<IConveyor> RepairCommand { get; private set; }

        public RelayCommand<IConveyor> ProcessCommand { get; private set; }

        public RelayCommand AddSugarCommand { get; private set; }

        public int SelectedConveyorIndex { get; set; }

        public MainViewModel()
        {
            DispatcherHelper.Initialize();

            StartCommand = new RelayCommand<IConveyor>(Start, CanStart);
            StopCommand = new RelayCommand<IConveyor>(Stop, CanStop);
            ProcessCommand = new RelayCommand<IConveyor>(Process, CanProcess);
            DeleteCommand = new RelayCommand<IConveyor>(Delete, CanDelete);
            RepairCommand = new RelayCommand<IConveyor>(Repair, CanRepair);
            AddCommand = new RelayCommand(Add, CanAdd);
            
            AddSugarCommand = new RelayCommand(AddSugar, CanAddSugar);
            
            Factory.OnSugarChanged += Factory_OnSugarChanged;
        }

        private void Factory_OnSugarChanged(IConveyor conveyor)
        {
            Application.Current.Dispatcher.Invoke(
              () =>
              {
                  RaisePropertyChanged(nameof(Factory));
                  Messenger.Default.Send(new SugarTakenMessage 
                  {
                      Conveyor = conveyor, 
                      ConveyorIndex = Conveyors.IndexOf(conveyor)
                  });
              },
              System.Windows.Threading.DispatcherPriority.Send);
        }

        public bool CanAdd()
        {
            return SelectedConveyorIndex >= 0;
        }

        public void Add()
        {
            var instance = (IConveyor)Activator.CreateInstance(ConveyorsOptions[SelectedConveyorIndex]);
            instance.OnCrashChanged += Instance_OnCrashChanged;
            Conveyors.Add(instance);
            RaisePropertyChanged(nameof(Conveyors));
        }

        private void Instance_OnCrashChanged(IConveyor obj)
        {
            Application.Current.Dispatcher.Invoke(
              () =>
              {
                  int index = Conveyors.IndexOf(obj);
                  Conveyors.Remove(obj);
                  Conveyors.Insert(index, obj);
                  RaisePropertyChanged(nameof(Conveyors));
              },
              System.Windows.Threading.DispatcherPriority.Send);
        }

        public bool CanDelete(IConveyor conveyor)
        {
            return !conveyor.IsStarted;
        }

        public void Delete(IConveyor conveyor)
        {
            conveyor.Stop();
            Conveyors.Remove(conveyor);
            RaisePropertyChanged(nameof(Conveyors));
        }

        public bool CanRepair(IConveyor conveyor)
        {
            return conveyor.IsCrashed;
        }

        public void Repair(IConveyor conveyor)
        {
            int index = Conveyors.IndexOf(conveyor);
            Conveyors.Remove(conveyor);
            conveyor.Repair();
            Conveyors.Insert(index, conveyor);
            RaisePropertyChanged(nameof(Conveyors));
        }

        public bool CanStart(IConveyor conveyor)
        {
            return !conveyor.IsStarted;
        }

        public void Start(IConveyor conveyor)
        {
            int index = Conveyors.IndexOf(conveyor);
            Conveyors.Remove(conveyor);
            conveyor.Start(Factory);
            Conveyors.Insert(index, conveyor);
            RaisePropertyChanged(nameof(Conveyors));
        }

        public bool CanStop(IConveyor conveyor)
        {
            return conveyor.IsStarted;
        }

        public void Stop(IConveyor conveyor)
        {
            int index = Conveyors.IndexOf(conveyor);
            Conveyors.Remove(conveyor);
            conveyor.Stop();
            Conveyors.Insert(index, conveyor);
            RaisePropertyChanged(nameof(Conveyors));
        }

        public bool CanAddSugar()
        {
            return true;
        }

        public void AddSugar()
        {
            Factory.SugarCount += 100;
            RaisePropertyChanged(nameof(Factory));
        }

        public bool CanProcess(IConveyor conveyor)
        {
            return true;
        }

        public async void Process(IConveyor conveyor)
        {
            await conveyor.Process(Factory);
            RaisePropertyChanged(nameof(Conveyors));
            RaisePropertyChanged(nameof(Factory));
        }
    }
}
